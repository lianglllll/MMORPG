using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player.PlayerState
{
    public class RemotePlayerState_Crouch: RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Crouch_Idle");

        }

        public override void Update()
        {

        }
    }
}
