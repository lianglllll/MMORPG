using HS.Protobuf.Scene;
using UnityEngine;

namespace Player
{
    public class RemotePlayerState_JumpUp: RemotePlayerState
    {
        private float m_verticalVelocity;
        private TransformChangeDate _latestDate = new();
        // private Vector3 deltaPosition = Vector3.zero;

        public override void Enter()
        {
            _latestDate.pos = remotePlayer.transform.position;
            _latestDate.rot = remotePlayer.transform.eulerAngles;
            // _latestDate.speed = remotePlayer.Actor.Speed;
            _latestDate.isCorrection = true;
            _latestDate.TimeStamp = 0;

            m_verticalVelocity = StateMachineParameter.jumpVelocity;
            remotePlayer.Model.Animator.SetFloat("Normal_Vertical_Speed", m_verticalVelocity);
            remotePlayer.PlayAnimation("Jump");
        }
        public override void Update()
        {
            QuickCorrection();

            m_verticalVelocity += remotePlayer.gravity * Time.deltaTime;
            remotePlayer.Model.Animator.SetFloat("Normal_Vertical_Speed", m_verticalVelocity);
        }

        public override void SyncTransformData(ActorChangeTransformDataResponse resp)
        {
            if (resp.Timestamp < _latestDate.TimeStamp)
            {
                goto End;
            }
            remotePlayer.NetworkActor.NV3ToV3(resp.OriginalTransform.Position, ref _latestDate.pos);
            remotePlayer.NetworkActor.NV3ToV3(resp.OriginalTransform.Rotation, ref _latestDate.rot);
            // _latestDate.speed = resp.PayLoad.VerticalSpeed * 0.001f;
            _latestDate.isCorrection = false;
            _latestDate.TimeStamp = resp.Timestamp;
            // remotePlayer.Model.Animator.SetFloat("Normal_Vertical_Speed", _latestDate.speed);
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
            // Vector3 target = new Vector3(_latestDate.pos.x, remotePlayer.transform.position.y, _latestDate.pos.z);
            Vector3 target = _latestDate.pos;
            if (Vector3.Distance(remotePlayer.transform.position, target) <= 0.01f)
            {
                isPosCorrenction = true;
                remotePlayer.transform.position = target;
            }
            else
            {
                remotePlayer.transform.position = Vector3.Lerp(remotePlayer.transform.position, target, Time.deltaTime * 5f);
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
