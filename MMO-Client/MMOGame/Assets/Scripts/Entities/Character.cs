using GameClient;
using GameClient.Entities;
using HS.Protobuf.SceneEntity;

namespace Assets.Script.Entities
{
    public class Character:Actor
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nCharacter"></param>
        public Character(NetActor nCharacter) : base(nCharacter)
        {

        }

        /// <summary>
        /// 处理死亡逻辑
        /// </summary>
        public override void OnDeath()
        {
            if (renderObj == null) return;
            base.OnDeath();

            if (GameApp.character == this)
            {
                GameApp._CombatPanelScript.ShowDeathBox();
                //主角死亡事件发生
                Kaiyun.Event.FireOut("CtlChrDeath");
            }
        }

        /// <summary>
        /// 经验值更新
        /// </summary>
        /// <param name="longValue1"></param>
        /// <param name="longValue2"></param>
        public void onExpChanged(long old_value, long new_value)
        {
            //更新当前actor的数据
            this.info.Exp = new_value;
            //事件通知，exp数据发送变化（可能某些ui组件需要这个信息）
            Kaiyun.Event.FireOut("ExpChange");
        }

        /// <summary>
        /// 金币更新
        /// </summary>
        /// <param name="longValue1"></param>
        /// <param name="longValue2"></param>
        public void onGoldChanged(long old_value, long new_value)
        {
            this.info.Gold = new_value;
            Kaiyun.Event.FireOut("GoldChange");
        }

    }

}
