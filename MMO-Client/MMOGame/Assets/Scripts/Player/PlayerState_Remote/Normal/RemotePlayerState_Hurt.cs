using GameClient.Entities;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class RemotePlayerState_Hurt : RemotePlayerState
    {
        public override void Enter()
        {
            remotePlayer.PlayAnimation("Hurt", true);
            LookAtTarget(StateMachineParameter.attacker);
        }

        private void LookAtTarget(Actor target)
        {
            if (target == null) return;
            //transform.LookAt(target.renderObj.transform.position);

            // 计算角色应该朝向目标点的方向
            Vector3 targetDirection = (target.RenderObj.transform.position - remotePlayer.transform.position).normalized;

            // 限制在Y轴上的旋转
            targetDirection.y = 0;

            // 计算旋转方向
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // 将角色逐渐旋转到目标方向
            //float rotationSpeed = 5f;
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 立即将角色转向目标方向
            remotePlayer.transform.rotation = targetRotation;
        }

    }
}
