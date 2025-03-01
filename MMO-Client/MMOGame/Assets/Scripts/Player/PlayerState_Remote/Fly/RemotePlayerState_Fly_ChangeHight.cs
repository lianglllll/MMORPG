using HS.Protobuf.SceneEntity;
using UnityEngine;

namespace Player.PlayerState
{
    public class RemotePlayerState_Fly_ChangeHight : RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Fly_ChangeHight");
        } 
    }
}
