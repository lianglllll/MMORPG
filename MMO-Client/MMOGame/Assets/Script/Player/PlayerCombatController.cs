using GameClient.Combat;
using GameClient.Entities;
using GameClient.Manager;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//连招信息
public class ComboData
{
    public Skill skill;
    public ComboData next;

    public ComboData()
    {
        skill = null;
        next = null;
    }

    public ComboData(Skill skill)
    {
        this.skill = skill;
        next = null;
    }

}


/// <summary>
/// 角色的战斗控制器
/// 现在主要是用来控制什么时候使用skill
/// </summary>
public class PlayerCombatController : MonoBehaviour
{
    private CtlStateMachine stateMachine;
    private SkillManager skillManager;
    private Actor owner;

    private Animator _animator;
    private float _attackColdTime;                          //下次发起攻击的间隔：skill吟唱+skill执行+skill后摇
    protected bool _applyAttackInput;                       //当前是否可以进行攻击

    //普通攻击连招表(全部连招表第一招都是空的，因为在攻击输入的时候会自动读取下一招)
    //普通攻击全是有目标技能
    private ComboData defaultComboData;
    private ComboData currentComboData;

    //敌人的范围检测
    private Actor _currentEnemy;
    protected float _detectionRange = 8.5f;
    private LayerMask _enemyLayer = 6;//actor


    private void Start()
    {
        _attackColdTime = 0f;
        _applyAttackInput = false;
    }

    private void Update()
    {
        SelectTargetObject();
        PlayerAttackInput();
        OnEndBaseAttack();
    }

    /// <summary>
    /// 组件初始化
    /// </summary>
    /// <param name="owner"></param>
    public void Init(Actor owner, CtlStateMachine stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
        this.skillManager = owner.skillManager;

        //初始化普通攻击连招表
        var baseSkillIds = owner.define.DefaultSkills;
        int count = baseSkillIds.Length;
        if (count <= 0) return;     //没有基础攻击就返回

        defaultComboData = new ComboData();
        ComboData lastComboData = defaultComboData;
        ComboData tmpComboData;
        for (int i = 0; i < count; ++i)
        {
            tmpComboData = new ComboData(skillManager.GetSkill(baseSkillIds[i]));
            lastComboData.next = tmpComboData;
            lastComboData = tmpComboData;
        }
        lastComboData.next = defaultComboData;

        _applyAttackInput = false;
    }



    /// <summary>
    /// 玩家攻击输入
    /// </summary>
    private void PlayerAttackInput()
    {
        if (!CanAttackInput()) return;

        //鼠标左键的base攻击
        if (GameInputManager.Instance.LAttack)             
        {
            //设置当前的combo信息
            SetComboData(currentComboData.next);
            BaseComboActionExecute();
        }


        //qefzxc等技能攻击

    }

    /// <summary>
    /// 是否可以攻击
    /// </summary>
    /// <returns></returns>
    private bool CanAttackInput()
    {

        if (_applyAttackInput == false) return false;
        if (stateMachine.currentEntityState == EntityState.Hit) return false;
        if (stateMachine.currentEntityState == EntityState.SkillActive) return false;
        if (stateMachine.currentEntityState == EntityState.SkillIntonate) return false;
        if (stateMachine.currentEntityState == EntityState.Dizzy) return false;
        if (stateMachine.currentEntityState == EntityState.Death) return false;
        //3. 角色正在格挡，不允许攻击
        //4.角色正在处决，不允许攻击
        return true;

    }

    /// <summary>
    /// 设置当前的连招信息
    /// </summary>
    /// <param name="next"></param>
    private void SetComboData(ComboData next)
    {
        if (next == null) return;
        currentComboData = next;
    }

    /// <summary>
    /// 执行普通攻击,发请求告知服务器
    /// 因为普通攻击的特殊性：当有锁定敌人的时候，我们需要到达攻击距离才能释放普通技能
    ///                     当没有锁定敌人的时候，我们就打自己，也就是原地放技能
    /// </summary>
    private void BaseComboActionExecute()
    {
        _applyAttackInput = false;
        //发请求给服务器，施法普通技能

        if( _currentEnemy == null || _currentEnemy == owner)
        {
            //尝试给自己获取一个最近的目标,如果没有成功就讲自己设置为敌人
            if (LockInRecentTargets() != 0)
            {
                _currentEnemy = owner;
                GameApp.target = owner;
            }
            else
            {
                //已经锁定了一个最近的目标，如果距离不够的话服务器会响应技能释放失败的
            }
        }

        CombatService.Instance.SpellSkill(currentComboData.skill,_currentEnemy);


        //设置允许下次攻击的时间间隔
        GameTimerManager.Instance.TryUseOneTimer(currentComboData.skill.GetAttackColdTime(), ()=> {
            //重置攻击输入标记
            _applyAttackInput = true;
        });
    }

    /// <summary>
    /// 攻击动作结束后，招式重置为第一招
    /// 基础连招->重置为基础连招
    /// 技能->重置为基础连招
    /// </summary>
    private void OnEndBaseAttack()
    {
        if(stateMachine.currentEntityState == EntityState.Motion && _applyAttackInput)
        {
            currentComboData = defaultComboData;
        }
    }



    /// <summary>
    /// 手动选择一个目标
    /// </summary>
    public void SelectTargetObject()
    {
        //选择目标
        if (Input.GetMouseButtonDown(0))  // 当鼠标左键被按下
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // 从鼠标点击位置发出一条射线
            RaycastHit hitInfo;  // 存储射线投射结果的数据
            LayerMask actorLayer = LayerMask.GetMask("Actor");
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, actorLayer))  // 检测射线是否与特定图层的物体相交
            {
                GameObject clickedObject = hitInfo.collider.gameObject;  // 获取被点击的物体，在这里可以对获取到的物体进行处理
                int entityId = clickedObject.GetComponent<GameEntity>().entityId;
                GameApp.target = EntityManager.Instance.GetEntity<Actor>(entityId);
                Kaiyun.Event.FireOut("SelectTarget");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearEnemy();
        }
    }

    /// <summary>
    /// 锁定最近的目标
    /// </summary>
    /// <returns></returns>
    protected int LockInRecentTargets()
    {
        //通过碰撞器获取周围的敌人
        Collider[] enemys = Physics.OverlapSphere(transform.position + (transform.up) * 0.7f,
            _detectionRange, _enemyLayer, QueryTriggerInteraction.Ignore);//没有collider的检测不到

        if (enemys == null) return -1;

        if (enemys.Length == 0) return -1;

        _currentEnemy = enemys[0].GetComponent<Actor>();
        GameApp.target = _currentEnemy;

        Kaiyun.Event.FireOut("SelectTarget");

        return 0;
    }

    /// <summary>
    /// 清除目标锁定的目标
    /// </summary>
    private void ClearEnemy()
    {
        GameApp.target = null;
        _currentEnemy = null;
        //触发一下取消敌人锁定的事件
        Kaiyun.Event.FireOut("CancelSelectTarget");
    }


}
