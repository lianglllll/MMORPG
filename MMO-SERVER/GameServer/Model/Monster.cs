using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using GameServer.Core;
using GameServer.AI;

namespace GameServer.Model
{
    public class Monster : Actor
    {
         
        public Vector3 moveTarget;//将要要移动的目标位置（tmp）
        public Vector3 movePosition;//当前移动中的位置(tmp)
        public Vector3 initPosition;//出生点
        public AIBase AI;
        private Random random = new Random();

        public Actor target;        //追击的目标
        private static Vector3Int Y1000 = new Vector3Int(0, 1000, 0);


        public Monster(int Tid,int level,Vector3Int position, Vector3Int direction) : base(EntityType.Monster, Tid,level, position, direction)
        {

            //设置专属monster的info
            //用tid获取一些基本信息，so character和moster势必是需要分表写的//todo


            //任务1 状态初始化
            initPosition = position;//出生点设置
            State = EntityState.Idle;

            //任务2,monster位置同步
            Scheduler.Instance.AddTask(() =>
            {
                if (State != EntityState.Move) return;
                //广播消息
                NEntitySync nEntitySync = new NEntitySync();
                nEntitySync.Entity = EntityData;
                nEntitySync.State = State;
                this.currentSpace.UpdateEntity(nEntitySync);
            }, 0.15f);

            //设置AI对象
            switch (Define.AI)
            {
                case "Monster":
                    this.AI = new MonsterAI(this);
                    break;
                default:
                    break;
            }

        }

        //计算出方向，客户端需要发送请求过来，经过服务端计算之后响应，客户端才能真正的移动
        public void MoveTo(Vector3 target)
        {
            if(this.State == EntityState.Idle)
            {
                State = EntityState.Move;//这个能触发下面的update
            }
            if(moveTarget != target)
            {
                moveTarget = target;
                movePosition = Position;
                var dir = (moveTarget - movePosition).normalized;//计算方向
                Direction = LookRotation(dir)* Y1000;
                //广播消息
                NEntitySync nEntitySync = new NEntitySync();
                nEntitySync.Entity = EntityData;
                nEntitySync.State = State;
                this.currentSpace.UpdateEntity(nEntitySync);
            }
        }

        //主要是计算服务端位移的数据，每秒50次
        public override void Update()
        {
            base.Update();      //技能更新
            AI?.Update();

            //monster移动实现
            if(State == EntityState.Move)
            {
                //移动方向
                var dir = (moveTarget - movePosition).normalized;
                this.Direction = LookRotation(dir)* Y1000;
                float dist = Speed * Time.deltaTime;
                if (Vector3.Distance(moveTarget, movePosition) < dist)
                {
                    StopMove();
                }
                else
                {
                    movePosition += dist * dir;
                }
                this.Position = movePosition;

            }
        }

        public void StopMove()
        {
            State = EntityState.Idle;
            movePosition = moveTarget;
            //广播消息
            NEntitySync nEntitySync = new NEntitySync();
            nEntitySync.Entity = EntityData;
            nEntitySync.State = State;
            this.currentSpace.UpdateEntity(nEntitySync);
        }

        //计算出生点附近的随机坐标
        public Vector3 RandomPointWithBirth(float range)
        {
            double x = random.NextDouble()*2f-1f;//[-1,1]
            double z = random.NextDouble()*2f- 1f;
            Vector3 dir = new Vector3(((float)x), 0, ((float)z)).normalized;
            return initPosition + dir * range * ((float)random.NextDouble());
        }
        
        //方向向量转换位欧拉角
        public Vector3 LookRotation(Vector3 fromDir)
        {
            float Rad2Deg = 57.29578f;
            Vector3 eulerAngles = new Vector3();

            // 计算欧拉角X
            eulerAngles.x = MathF.Acos(MathF.Sqrt((fromDir.x * fromDir.x + fromDir.z * fromDir.z) / (fromDir.x * fromDir.x + fromDir.y * fromDir.y + fromDir.z * fromDir.z))) * Rad2Deg;
            if (fromDir.y > 0)
                eulerAngles.x = 360 - eulerAngles.x;

            // 计算欧拉角Y
            eulerAngles.y = MathF.Atan2(fromDir.x, fromDir.z) * Rad2Deg;
            if (eulerAngles.y < 0)
                eulerAngles.y += 180;
            if (fromDir.x < 0)
                eulerAngles.y += 180;

            // 欧拉角Z为0
            eulerAngles.z = 0;

            return eulerAngles;
        }
    }
}
 