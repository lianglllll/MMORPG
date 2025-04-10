using GameClient;
using GameClient.Entities;
using HS.Protobuf.SceneEntity;

namespace GameClient.Entities
{
    public class Character : Actor
    {
        public Character(NetActorNode nCharacter) : base(nCharacter)
        {
        }

        public override void OnDeath()
        {
            if (RenderObj == null) return;

            //如果当前actor被关注，则需要通知
            if (GameApp.target == this)
            {
                Kaiyun.Event.FireIn("TargetDeath");
            }else if (GameApp.character == this)
            {
                GameApp.CombatPanelScript.ShowDeathBox();
                //主角死亡事件发生
                Kaiyun.Event.FireOut("CtlChrDeath");
            }
        }
    }
}
