using GameClient.Combat;
using GameClient.Entities;
using GameClient.Manager;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 连招信息
/// </summary>
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
    private PlayerStateMachine stateMachine;
    private SkillManager skillManager;
    private Actor owner;

    protected bool _applyAttackInput => GameApp.CurrSkill == null;               //当前是否可以进行攻击

    //普通攻击连招表(全部连招表第一招都是空的，因为在攻击输入的时候会自动读取下一招)
    //普通攻击全是有目标技能
    private ComboData defaultComboData;
    private ComboData currentComboData;
    private bool isComboBufferTime;
    private float remainComboBufferTime;
    private float comboBufferTime = 2f;

    //敌人的范围检测
    private Actor _currentEnemy;
    protected float _detectionRange = 15f;
    private LayerMask _enemyLayer;

    //主动技能
    private Dictionary<char,Skill> ActiveTypeSkills  = new();



    private void Start()
    {
        _enemyLayer = LayerMask.GetMask("Actor");
    }

    private void Update()
    {
        SelectTargetObject();
        PlayerAttackInput();
        ComboEnd();
        ClearEnemyWhenMotion();
    }

    private void OnEnable()
    {
        //目标死亡
        Kaiyun.Event.RegisterIn("TargetDeath", this, "ClearEnemy");
        Kaiyun.Event.RegisterOut("CtlChrDeath", this, "ClearEnemy");
        Kaiyun.Event.RegisterIn("SkillActiveEnd", this, "SkillActiveEnd");

    }

    private void OnDisable()
    {
        Kaiyun.Event.UnregisterIn("TargetDeath", this, "ClearEnemy");
        Kaiyun.Event.UnregisterOut("CtlChrDeath", this, "ClearEnemy");
        Kaiyun.Event.UnregisterIn("SkillActiveEnd", this, "SkillActiveEnd");
    }


    /// <summary>
    /// 组件初始化
    /// </summary>
    /// <param name="owner"></param>
    public void Init(Actor owner)
    {
        this.owner = owner;
        this.stateMachine = owner.StateMachine;
        this.skillManager = owner.skillManager;

        //初始化普通攻击连招表
        var baseSkillIds = owner.define.DefaultSkills;
        int count = baseSkillIds.Length;
        if (count <= 0) return;     //没有基础攻击就返回

        defaultComboData = new ComboData();
        ComboData lastComboData = defaultComboData;
        ComboData tmpComboData;
        foreach (var skill in skillManager.GetCommonSkills())
        {
            tmpComboData = new ComboData(skill);
            lastComboData.next = tmpComboData;
            lastComboData = tmpComboData;
        }
        lastComboData.next = defaultComboData.next;
        currentComboData = defaultComboData;

        //初始化主动技能
        string Keys = "QEFZXC";
        int index = 0;
        foreach(var skill in skillManager.GetActiveSkills())
        {
            ActiveTypeSkills.Add(Keys[index], skill);
            index++;
        }

    }


    /// <summary>
    /// 玩家攻击输入
    /// </summary>
    private void PlayerAttackInput()
    {
        if (!CanAttackInput())
        {
            return;
        }

        //鼠标左键的base攻击
        if (Input.GetKeyDown(KeyCode.Alpha1) )            
        {
            SetComboData(currentComboData.next);
            BaseComboActionExecute();
            //施法范围圈
            owner.unitUIController.SetSpellRangeCanvas(true, currentComboData.skill.Define.SpellRange * 0.001f);
            GameTimerManager.Instance.TryUseOneTimer(0.1f, () =>
            {
                //关闭攻击范围
                owner.unitUIController.SetSpellRangeCanvas(false);
            });
        }

        //qefzxc等技能攻击
        SkillActionExecute();

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
        //发请求给服务器，施法普通技能

        if( _currentEnemy == null || _currentEnemy == owner)
        {
            //尝试给自己获取一个最近的目标,如果没有成功就讲自己设置为敌人
            if (LockInRecentTargets() != 0)
            {
                CombatService.Instance.SpellSkill(currentComboData.skill, owner);
                return;
            }
            else
            {
                //已经锁定了一个最近的目标，计算当前的攻击距离
                if(Vector3.Distance(transform.position,_currentEnemy.renderObj.transform.position) > currentComboData.skill.Define.SpellRange * 0.001f)
                {
                    //打不到敌人，自己原地平啊
                    //获取移动到可以攻击到敌人的位置

                    //transform.LookAt(_currentEnemy.renderObj.transform.position);
                    LookAtTarget(_currentEnemy);

                    CombatService.Instance.SpellSkill(currentComboData.skill, owner);
                    return;
                }

                //剩下就是打敌人的了

            }
        }
        else
        {
            //已经锁定了一个最近的目标，计算当前的攻击距离
            if (Vector3.Distance(transform.position, _currentEnemy.renderObj.transform.position) > currentComboData.skill.Define.SpellRange *0.001f)
            {
                //打不到敌人，自己原地平啊
                //获取移动到可以攻击到敌人的位置
                //transform.LookAt(_currentEnemy.renderObj.transform.position);
                LookAtTarget(_currentEnemy);
                CombatService.Instance.SpellSkill(currentComboData.skill, owner);
                return;

            }

            //剩下就是打敌人的了

        }

        LookAtTarget(_currentEnemy);
        CombatService.Instance.SpellSkill(currentComboData.skill,_currentEnemy);

    }

    /// <summary>
    /// 攻击动作结束后，招式重置为第一招
    /// 基础连招->重置为基础连招
    /// 技能->重置为基础连招
    /// </summary>
    public void SkillActiveEnd()
    {
        //开启连招缓存时间
        remainComboBufferTime = comboBufferTime;
        isComboBufferTime = true;
    }

    /// <summary>
    /// 连招结束，重置普通攻击
    /// </summary>
    private void ComboEnd()
    {
        if (isComboBufferTime)
        {
            remainComboBufferTime -= Time.deltaTime;
            if(remainComboBufferTime <= 0f)
            {
                currentComboData = defaultComboData;
                isComboBufferTime = false;
            }
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
                ClearEnemy();
                LockTarget(clickedObject.GetComponent<GameEntity>().owner);
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
        if (enemys.Length <= 1) return -1;

        for(int i = 0; i < enemys.Length; ++i)
        {
            Actor tmpA = enemys[i].GetComponent<GameEntity>().owner;
            if (!tmpA.IsDeath && tmpA != owner)
            {
                LockTarget(tmpA);
                return 0;
            }
        }

        return -1;
    }

    /// <summary>
    /// 锁定目标
    /// </summary>
    /// <param name="target"></param>
    public void LockTarget(Actor target)
    {
        _currentEnemy = target;
        GameApp.target = _currentEnemy;
        _currentEnemy.unitUIController.SetSelectMark(true);
        Kaiyun.Event.FireOut("SelectTarget");//ui
    }

    /// <summary>
    /// 清除目标锁定的目标
    /// </summary>
    public void ClearEnemy()
    {
        if (_currentEnemy == null || GameApp.target == null) return;
        //触发一下取消敌人锁定的事件
        Kaiyun.Event.FireOut("CancelSelectTarget");//ui
        _currentEnemy?.unitUIController.SetSelectMark(false);
        GameApp.target = null;
        _currentEnemy = null;
    }

    private void ClearEnemyWhenMotion()
    {
        if(_currentEnemy !=null && stateMachine.currentEntityState == EntityState.Motion && 
            Vector3.Distance(transform.position,_currentEnemy.renderObj.transform.position) > _detectionRange)
        {
            ClearEnemy();
        }
    }


    /// <summary>
    /// 释放技能
    /// </summary>
    public void SkillActionExecute()
    {
        if (_applyAttackInput == false) return;

        Skill skill;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActiveTypeSkills.TryGetValue('Q', out  skill);
            if (skill == null) return;

        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ActiveTypeSkills.TryGetValue('E', out skill);
            if (skill == null) return;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            ActiveTypeSkills.TryGetValue('F', out skill);
            if (skill == null) return;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ActiveTypeSkills.TryGetValue('Z', out skill);
            if (skill == null) return;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ActiveTypeSkills.TryGetValue('X', out skill);
            if (skill == null) return;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            ActiveTypeSkills.TryGetValue('C', out skill);
            if (skill == null) return;
        }
        else
        {
            return;
        }

        if (skill.IsUnitTarget && GameApp.target == null)
        {
            UIManager.Instance.MessagePanel.ShowBottonMsg("当前没有选中目标");
            return;
        }

        CombatService.Instance.SpellSkill(skill, _currentEnemy);

        //施法范围圈
        owner.unitUIController.SetSpellRangeCanvas(true, skill.Define.SpellRange * 0.001f);
        GameTimerManager.Instance.TryUseOneTimer(0.1f, () =>
        {
            //关闭攻击范围
            owner.unitUIController.SetSpellRangeCanvas(false);
        });

    }


    /// <summary>
    /// 展示技能指示器
    /// </summary>
    public void ShowSpellRangeUI(Skill skill)
    {
        //技能指示器，最起码有一个释放距离的圈圈
        //现在先不用这个玩意
        //我们显示一个技能释放距离的圈圈就行了


    }

    /// <summary>
    /// 看向目标
    /// </summary>
    /// <param name="target"></param>
    public void LookAtTarget(Actor target)
    {
        if (target == null) return;
        //transform.LookAt(target.renderObj.transform.position);

        // 计算角色应该朝向目标点的方向
        Vector3 targetDirection = target.renderObj.transform.position - transform.position;

        // 限制在Y轴上的旋转
        targetDirection.y = 0;

        // 计算旋转方向
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // 将角色逐渐旋转到目标方向
        //float rotationSpeed = 5f;
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 立即将角色转向目标方向
        transform.rotation = targetRotation;

    }



}
