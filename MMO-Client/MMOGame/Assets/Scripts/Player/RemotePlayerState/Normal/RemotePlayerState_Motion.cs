using HS.Protobuf.Scene;
using Serilog;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class MotionChangeDate
    {
        public Vector3 pos;
        public Vector3 rot;
        public float speed;
        public bool isCorrection;   // 是否已经纠正了
        public long TimeStamp;
    }


    public class RemotePlayerState_Motion:RemotePlayerState
    {
        private MotionChangeDate _latestDate = new();

        public override void Enter()
        {
            _latestDate.pos = remotePlayer.transform.position;
            _latestDate.rot = remotePlayer.transform.eulerAngles;
            _latestDate.speed = remotePlayer.Actor.Speed;
            _latestDate.isCorrection = true;
            _latestDate.TimeStamp = 0;
            remotePlayer.PlayAnimation("Motion");
        }
        public override void Update()
        {
            Interpolation();
            QuickCorrection();
            Prediction();
            //重力
            remotePlayer.CharacterController.Move(new Vector3(0, remotePlayer.gravity * Time.deltaTime, 0));
        }
        public override void Exit()
        {
        }

        public void AddNetworkState(ActorChangeMotionDataResponse resp)
        {
            if (resp.Timestamp < _latestDate.TimeStamp)
            {
                goto End;
            }
            remotePlayer.NetworkActor.NV3ToV3(resp.OriginalTransform.Position, ref _latestDate.pos);
            remotePlayer.NetworkActor.NV3ToV3(resp.OriginalTransform.Rotation, ref _latestDate.rot);
            _latestDate.speed = resp.Speed;
            _latestDate.isCorrection = false;
            _latestDate.TimeStamp = resp.Timestamp;
        End:
            return;
        }
        private void Interpolation()
        {

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
            Vector3 taret = new Vector3(_latestDate.pos.x, remotePlayer.transform.position.y, _latestDate.pos.z);
            if(Vector3.Distance(remotePlayer.transform.position, taret) < 0.01f)
            {
                isPosCorrenction = true;
                remotePlayer.transform.position = taret;
            }
            else
            {
                remotePlayer.transform.position = Vector3.Lerp(remotePlayer.transform.position, taret, Time.deltaTime * 5f);
            }

            // 旋转四元数，插值处理
            if(Quaternion.Angle(remotePlayer.transform.rotation, Quaternion.Euler(_latestDate.rot)) < 1f)
            {
                isRotCorrenction = true;
                remotePlayer.transform.rotation = Quaternion.Euler(_latestDate.rot);
            }
            else
            {
                Quaternion targetQuaternion = Quaternion.Euler(_latestDate.rot);
                remotePlayer.transform.rotation = Quaternion.Lerp(remotePlayer.transform.rotation, targetQuaternion, Time.deltaTime * 5f);
            }

            if(isPosCorrenction && isRotCorrenction)
            {
                _latestDate.isCorrection = true;
                Log.Information("QuickCorrection 成功");
            }

        End:
            return;
        }
        private void Prediction()
        {
            if(!_latestDate.isCorrection)
            {
                goto End;
            }

            // 使用角色的当前朝向进行航位推算
            Vector3 moveDirection = remotePlayer.transform.forward;
            // 只在水平面移动（忽略Y轴方向）
            moveDirection.y = 0;
            moveDirection.Normalize();

            // 计算帧间移动量
            float predictedDistance = _latestDate.speed * Time.deltaTime;
            Vector3 predictedMovement = moveDirection * predictedDistance;

            // 使用CharacterController进行带碰撞检测的移动
            remotePlayer.CharacterController.Move(predictedMovement);

            // （可选）同步旋转状态（如果旋转可能独立调整）
            // remotePlayer.transform.rotation = Quaternion.Euler(_latestDate.rot);

        End:
            return;
        }
    }
}
