using HSFramework.MyDelayedTaskScheduler;
using GameClient;
using GameClient.Entities;
using HS.Protobuf.SceneEntity;

namespace GameClient.Entities
{
    public class Monster : Actor
    {
        public Monster(NetActorNode nCharacter) : base(nCharacter)
        {

        }

        public override void OnDeath()
        {
            if (m_renderObj == null) return;

            //如果当前actor被关注，则需要通知
            if (GameApp.target == this)
            {
                Kaiyun.Event.FireIn("TargetDeath");
            }

            //隐藏怪物实体
            DelayedTaskScheduler.Instance.AddDelayedTask(3f, () =>
            {
                //如果单位死亡，将其隐藏
                //这里判断是防止在死亡的3秒内本actor复活了
                if (IsDeath)
                {
                    m_renderObj?.SetActive(false);
                }
            });
        }

        //public void OnRevive()
        //{
        //    m_renderObj?.SetActive(true);
        //}
    }
}
