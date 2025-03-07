using System;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public abstract class ModelBase : MonoBehaviour
    {
        protected Animator animator;
        public Animator Animator { get => animator; }
        private BaseController m_baseController;


        protected void Awake()
        {
            animator = GetComponent<Animator>();
        }

        //初始化
        public void Init(BaseController baseController)
        {
            m_baseController = baseController;
        }

        #region 根运动

        protected Action<Vector3, Quaternion> rootMotionAction;

        public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction)
        {
            this.rootMotionAction = rootMotionAction;
        }

        public void ClearRootMotionAction()
        {
            rootMotionAction = null;
        }

        protected void OnAnimatorMove()
        {
            //这一帧运动的位移和旋转
            rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
        }

        #endregion

        #region 动画事件

        protected void FootStep()
        {
            m_baseController.OnFootStep();
        }

        protected void StartSkillHit(int weaponIndex)
        {
        }

        protected void StopSkillHit(int weaponIndex)
        {
        }

        //允许变招
        protected void CanSwitchSkill()
        {
        }

        //允许打断
        protected void CanCancelSkill()
        {
        }

        //技能结束
        protected void SkillOver()
        {
        }

        #endregion


    }
}
