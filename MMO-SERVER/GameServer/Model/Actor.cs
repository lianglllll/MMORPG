using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using GameServer.Core;
using GameServer.Combat;
using GameServer.Manager;

namespace GameServer.Model
{

    public class Actor : Entity
    {
        public UnitDefine Define;                                                                   //怪物中：山贼，土匪？
        public NetActor info  = new NetActor();                                                     //作为存放网络信息的tmp
        public EntityState State;                                                                   //actor状态
        public Space currentSpace;                                                                  //todo,可以修改space的同时去修改info.spaceid
        public bool IsDeath;                                                                        //是否死亡，默认false
        public Attributes Attr = new Attributes();                                                  //actor属性
        public SkillManager skillManager;                                                           //actor技能管理器
        public Spell spell;                                                                         //actor技能释放器




        public int Id { 
            get { return info.Id; } 
            set { info.Id = value; } 
        }                                                                           //actorId
        public string Name { 
            get { return info.Name; } 
            set { info.Name = value; } 
        }
        public EntityType Type { 
            get { return info.EntityType; } 
            set { info.EntityType = value; }
        }                                                                  //角色，怪物，npc？
        public int SpaceId
        {
            get
            {
                return currentSpace.SpaceId;
            }
        }


        //TID:区分相同实体类型，不同身份。可以通过tid去找define
        public Actor( EntityType type,int TID,int level,Vector3Int position, Vector3Int direction) : base(position, direction)
        {
            //加载define的默认数据
            this.Define = DataManager.Instance.unitDefineDict[TID];//todo 可能会空，可能会有人篡改json配置文件
            this.info.Tid = TID;
            this.info.EntityType = type;
            this.info.Entity = this.EntityData;
            this.info.Name = Define.Name;               //默认名，可以给子类进行覆盖
            this.info.Hp = (int)this.Define.HPMax;
            this.info.Mp = (int)this.Define.MPMax;
            this.info.Level = level;
            this.Speed = this.Define.Speed;
            

            this.skillManager = new SkillManager(this);
            this.Attr.Init(this);
            this.spell = new Spell(this);
        }

        public override void Update()
        {
            this.skillManager.Update();//驱动技能系统
        }

        //actor进入场景
        public void OnEnterSpace(Space space)
        {
            this.currentSpace = space;
            this.info.SpaceId = space.SpaceId;
        }

        //复活
        public void Revive()
        {
            this.IsDeath = false;
        }

    }
}
