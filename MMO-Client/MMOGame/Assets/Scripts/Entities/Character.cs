using GameClient;
using GameClient.Entities;
using HS.Protobuf.SceneEntity;

namespace Assets.Script.Entities
{
    public class Character:Actor
    {
        public Character(NetActorNode nCharacter) : base(nCharacter)
        {
        }

        public override void OnDeath()
        {
            if (m_renderObj == null) return;
            base.OnDeath();

            if (GameApp.character == this)
            {
                GameApp._CombatPanelScript.ShowDeathBox();
                //主角死亡事件发生
                Kaiyun.Event.FireOut("CtlChrDeath");
            }
        }
        public void OnExpChanged(long old_value, long new_value)
        {
            //更新当前actor的数据
            this.m_netActorNode.Exp = new_value;
            //事件通知，exp数据发送变化（可能某些ui组件需要这个信息）
            Kaiyun.Event.FireOut("ExpChange");
        }
        public void OnGoldChanged(long old_value, long new_value)
        {
            this.m_netActorNode.Gold = new_value;
            Kaiyun.Event.FireOut("GoldChange");
        }
    }

}
