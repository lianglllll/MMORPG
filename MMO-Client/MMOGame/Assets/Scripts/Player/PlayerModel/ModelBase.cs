using System;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class ModelBase : MonoBehaviour
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

        protected Action<int> m_startSkillHitAction;
        protected Action<int> m_stopSkillHitAction;

        public void SetSkillHitAction(Action<int> startSkillHitAction, Action<int> stopSkillHitAction)
        {
            m_startSkillHitAction = startSkillHitAction;
            m_stopSkillHitAction = stopSkillHitAction;
        }
        public void ClearSkillHitAction()
        {
            m_startSkillHitAction = null;
            m_stopSkillHitAction = null;
        }

        protected void FootStep()
        {
            m_baseController.OnFootStep();
        }

        protected void StartSkillHit(int weaponIndex)
        {
            m_startSkillHitAction?.Invoke(weaponIndex);
        }

        protected void StopSkillHit(int weaponIndex)
        {
            m_stopSkillHitAction?.Invoke(weaponIndex);
        }

        //允许变招
        protected void CanSwitchSkill()
        {
            m_baseController.OnCanSwitchSkill();
        }

        //允许打断
        protected void CanCancelSkill()
        {
            m_baseController.OnCanCancelSkill();
        }

        //技能结束
        protected void SkillOver()
        {
        }

        protected void EquipAction()
        {
            m_baseController.EquipAction();
        }
        protected void EquipEndAction()
        {
            m_baseController.EquipEndAction();

        }
        protected void UnEquipAction()
        {
            m_baseController.UnEquipAction();

        }
        protected void UnEquipEndAction()
        {
            m_baseController.UnEquipEndAction();
        }
        #endregion


    }
}
