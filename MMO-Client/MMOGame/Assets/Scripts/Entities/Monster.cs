using BaseSystem.MyDelayedTaskScheduler;
using GameClient.Entities;
using HS.Protobuf.SceneEntity;

namespace Assets.Script.Entities
{
    public class Monster : Actor
    {
        public Monster(NetActor nCharacter) : base(nCharacter)
        {

        }

        public override void OnDeath()
        {
            if (renderObj == null) return;
            base.OnDeath();
            //隐藏怪物实体
            DelayedTaskScheduler.Instance.AddDelayedTask(3f, () =>
            {
                //如果单位死亡，将其隐藏
                //这里判断是防止在死亡的3秒内本actor复活了
                if (IsDeath)
                {
                    renderObj?.SetActive(false);
                }
            });
        }

        /// <summary>
        /// 复活
        /// </summary>
        public void OnRevive()
        {
            renderObj?.SetActive(true);

        }
    }
}
