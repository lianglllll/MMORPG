using HS.Protobuf.Scene;
using UnityEngine;

namespace Player.PlayerState
{
    public class RemotePlayerState_Fly_ChangeHight : RemotePlayerState
    {
        private TransformChangeDate _latestDate = new();

        public override void Enter()
        {
            _latestDate.pos = remotePlayer.transform.position;
            _latestDate.rot = remotePlayer.transform.eulerAngles;
            _latestDate.verticalSpeed = remotePlayer.Actor.Speed;
            _latestDate.isCorrection = true;
            _latestDate.TimeStamp = 0;
            remotePlayer.PlayAnimation("Fly_ChangeHight");
        }
        public override void Update()
        {
            QuickCorrection();
        }

        public override void SyncTransformData(ActorChangeTransformDataResponse resp)
        {
            if (resp.Timestamp < _latestDate.TimeStamp)
            {
                goto End;
            }
            remotePlayer.NetworkActor.NV3ToV3(resp.OriginalTransform.Position, ref _latestDate.pos);
            remotePlayer.NetworkActor.NV3ToV3(resp.OriginalTransform.Rotation, ref _latestDate.rot);
            _latestDate.hightSpeed = resp.PayLoad.HightSpeed * 0.001f;
            _latestDate.isCorrection = false;
            _latestDate.TimeStamp = resp.Timestamp;
            remotePlayer.Model.Animator.SetFloat("Fly_Hight_Speed", _latestDate.hightSpeed);
        End:
            return;
        }
        private void QuickCorrection()
        {
            if (_latestDate.isCorrection)
            {
                goto End;
            }

            bool isPosCorrenction = false;
            bool isRotCorrenction = false;

            // 位置插值处理
            //Vector3 taret = new Vector3(_latestDate.pos.x, remotePlayer.transform.position.y, _latestDate.pos.z);
            Vector3 taret = _latestDate.pos;
            if (Vector3.Distance(remotePlayer.transform.position, taret) <= 0.01f)
            {
                isPosCorrenction = true;
                remotePlayer.transform.position = taret;
            }
            else
            {
                remotePlayer.transform.position = Vector3.Lerp(remotePlayer.transform.position, taret, Time.deltaTime * 5f);
            }

            // 旋转四元数，插值处理
            if (Quaternion.Angle(remotePlayer.transform.rotation, Quaternion.Euler(_latestDate.rot)) <= 1f)
            {
                isRotCorrenction = true;
                remotePlayer.transform.rotation = Quaternion.Euler(_latestDate.rot);
            }
            else
            {
                Quaternion targetQuaternion = Quaternion.Euler(_latestDate.rot);
                remotePlayer.transform.rotation = Quaternion.Lerp(remotePlayer.transform.rotation, targetQuaternion, Time.deltaTime * 5f);
            }

            if (isPosCorrenction && isRotCorrenction)
            {
                _latestDate.isCorrection = true;
            }

        End:
            return;
        }
    }
}
