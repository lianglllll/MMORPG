using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem.Tool
{
    public class DevelopmentToos
    {

        /// <summary>
        /// 不受帧数影响的Lerp
        /// </summary>
        /// <param name="time">平滑时间(尽量设置为大于10的值)</param>
        public static float UnTetheredLerp(float time = 10f)
        {
            return 1 - Mathf.Exp(-time * Time.deltaTime);
        }

        /// <summary>
        /// 取目标方向(返回一个标量)
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="self">自身</param>
        /// <returns></returns>
        public static Vector3 DirectionForTarget(Transform target, Transform self)
        {
            return (self.position - target.position).normalized;
        }

        /// <summary>
        /// 返回于目标之间的距离
        /// </summary>
        /// <param name="target"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float DistanceForTarget(Transform target, Transform self)
        {
            return Vector3.Distance(self.position, target.position);
        }

        /// <summary>
        /// 获取增量角（其实就是两个方向的角度差）
        /// </summary>
        /// <param name="currentDirection">当前移动方向</param>
        /// <param name="targetDirection">目标移动方向</param>
        /// <returns></returns>
        public static float GetDeltaAngle(Transform currentDirection, Vector3 targetDirection)
        {
            //当前角色朝向的角度
            float angleCurrent = Mathf.Atan2(currentDirection.forward.x, currentDirection.forward.z) * Mathf.Rad2Deg;
            //目标方向的角度也就是希望角色转过去的那个方向的角度
            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

            return Mathf.DeltaAngle(angleCurrent, targetAngle);
        }

        /// <summary>
        /// 计算当前朝向于目标方向之间的夹角
        /// </summary>
        /// <param name="target"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float GetAngleForTargetDirection(Transform target, Transform self)
        {
            return Vector3.Angle(((self.position - target.position).normalized), self.forward);
        }

        /// <summary>
        /// 限制一个值或者度数在-360-360之间
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float ClampValueOn360(float f)
        {
            f %= 360f;
            if (f < 0)
                f += 360;

            return f;
        }

        /// <summary>
        /// 限制一个值或者度数在-180-180之间
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float ClampValueOn180(float f)
        {
            f = (f + 180f) % 360f - 180f;

            if (f < -180)
                f += 360;

            return f;
        }

        /// <summary>
        /// 从当前位置移动到目标位置
        /// 计算当前点和目标点之间的位置，移动不超过maxDistanceDelta指定的距离。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3 TargetPositionOffset(Transform target, Transform self, float time)
        {
            var pos = target.transform.position;
            return Vector3.MoveTowards(self.position, pos, UnTetheredLerp(time));
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="message"></param>
        public static void WTF(object message)
        {
            Debug.LogFormat($"日志内容:<color=#ff0000> --->   {message}   <--- </color>");
        }
    }
}
