using GameClient;
using GameClient.Combat;
using GameClient.Entities;
using GameClient.Manager;
using Player;
using Player.Controller;
using Proto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private bool m_IsStart;
    private CtrlController ctrlController;
    private SkillManager skillManager;

    protected bool _applyAttackInput => GameApp.CurrSkill == null;               //当前是否可以进行攻击输入

    //普通攻击连招表(全部连招表第一招都是空的，因为在攻击输入的时候会自动读取下一招)
    //普通攻击全是有目标技能
    private ComboData defaultComboData;
    private ComboData currentComboData;
    private bool isComboBufferTime;
    private float remainComboBufferTime;
    private float comboBufferTime = 2f;

    //敌人的范围检测
    private Actor owner => ctrlController.Actor;
    private Actor _currentEnemy;
    protected float _detectionRange = 15f;
    private LayerMask _enemyLayer;

    //主动技能字典
    private Dictionary<KeyCode,Skill> ActiveTypeSkills  = new();
    protected Skill selectedSkill;                                              //当前输入的主动技能

    private void Start()
    {
        _enemyLayer = LayerMask.GetMask("Actor");
    }
    private void Update()
    {
        if (!m_IsStart) return;
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
    public void Init(CtrlController ctrlController)
    {
        this.ctrlController = ctrlController;
        this.skillManager = ctrlController.Actor.skillManager;

        //初始化普通攻击连招表
        var baseSkillIds = ctrlController.Actor.define.DefaultSkills;
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
        List<KeyCode> Keys = new List<KeyCode>();
        Keys.Add(KeyCode.Q);
        Keys.Add(KeyCode.E);
        Keys.Add(KeyCode.F);
        Keys.Add(KeyCode.Z);
        Keys.Add(KeyCode.X);
        Keys.Add(KeyCode.C);

        int index = 0;
        foreach(var skill in skillManager.GetActiveSkills())
        {
            ActiveTypeSkills.Add(Keys[index], skill);
            index++;
        }

        m_IsStart = true;
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
        if (Input.GetKeyDown(KeyCode.Alpha1))            
        {
            //施法范围圈
            ctrlController.unitUIController.SetSpellRangeCanvas(true, currentComboData.next.skill.Define.SpellRangeRadius * 0.001f);
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SetComboData(currentComboData.next);
            BaseComboActionExecute();
            //关闭攻击范围
            ctrlController.unitUIController.SetSpellRangeCanvas(false);
        }

        //qefzxc等技能攻击
        SkillInput();

    }

    /// <summary>
    /// 是否可以攻击
    /// </summary>
    /// <returns></returns>
    private bool CanAttackInput()
    {
        if (_applyAttackInput == false) return false;
        if (ctrlController.CurState == ActorState.Hurt) return false;
        if (ctrlController.CurState == ActorState.Skill) return false;
        if (ctrlController.CurState == ActorState.Dizzy) return false;
        if (ctrlController.CurState == ActorState.Death) return false;
        if (ctrlController.CurState == ActorState.Defense) return false;
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

        //没有目标
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
                if(Vector3.Distance(transform.position,_currentEnemy.renderObj.transform.position) > currentComboData.skill.Define.SpellRangeRadius * 0.001f)
                {
                    //打不到敌人，自己原地平啊
                    //获取移动到可以攻击到敌人的位置
                    LookAtTarget(_currentEnemy);
                    CombatService.Instance.SpellSkill(currentComboData.skill, owner);
                    return;
                }

                //剩下就是打敌人的了

            }
        }
        //有目标
        else
        {
            //已经锁定了一个最近的目标，计算当前的攻击距离
            if (Vector3.Distance(transform.position, _currentEnemy.renderObj.transform.position) > currentComboData.skill.Define.SpellRangeRadius * 0.001f)
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
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())  // 当鼠标左键被按下
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // 从鼠标点击位置发出一条射线
            RaycastHit hitInfo;  // 存储射线投射结果的数据
            LayerMask actorLayer = LayerMask.GetMask("Actor");
            LayerMask groundLayer = LayerMask.GetMask("Ground");
            LayerMask combinedLayer = actorLayer | groundLayer;  // 合并两个LayerMask
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, combinedLayer))  // 检测射线是否与特定图层的物体相交
            {
                // 获取被射线击中的碰撞体
                Collider collider = hitInfo.collider;

                // 检查碰撞体所在的层级
                if (collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
                {
                    GameObject clickedObject = hitInfo.collider.gameObject;  // 获取被点击的物体，在这里可以对获取到的物体进行处理
                    LockTarget(clickedObject.GetComponent<BaseController>().Actor);
                    Kaiyun.Event.FireOut("SelectTarget");
                }
                else if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    ctrlController.MoveToPostion(hitInfo.point);
                }
                else
                {

                }
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

        Actor targetActor = null;
        float minDistance = float.MaxValue;
        for (int i = 0; i < enemys.Length; ++i)
        {
            var tmpA = enemys[i].GetComponent<BaseController>().Actor;
            if (owner == tmpA || tmpA.IsDeath) continue;
            var dis = Vector3.Distance(transform.position,tmpA.renderObj.transform.position);
            if(dis < minDistance)
            {
                targetActor = tmpA;
                minDistance = dis;
            }
        }

        if (targetActor != null)
        {
            LockTarget(targetActor);
            return 0;
        }

        return -1;
    }

    /// <summary>
    /// 锁定目标
    /// </summary>
    /// <param name="target"></param>
    public void LockTarget(Actor target)
    {
        if (target == null) return;
        ClearEnemy();
        _currentEnemy = target;
        GameApp.target = _currentEnemy;
        _currentEnemy.baseController.unitUIController.SetSelectedMark(true);
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
        _currentEnemy.baseController.unitUIController.SetSelectedMark(false);

        GameApp.target = null;
        _currentEnemy = null;
    }

    /// <summary>
    /// 角色移动出一定的范围就取消锁定
    /// </summary>
    private void ClearEnemyWhenMotion()
    {
        if(_currentEnemy !=null && ctrlController.CurState == ActorState.Move && 
            Vector3.Distance(transform.position,_currentEnemy.renderObj.transform.position) > _detectionRange)
        {
            ClearEnemy();
        }
    }

    /// <summary>
    /// 技能输入
    /// </summary>
    public void SkillInput()
    {

        //按下
        foreach(var key in ActiveTypeSkills.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                SkillKeyDown(key);
                break;
            }
        }

        //抬起
        foreach (var key in ActiveTypeSkills.Keys)
        {
            if (Input.GetKeyUp(key))
            {
                SkillKeyUp(key);
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CloseSpellRangeUI();
        }

    }

    public void SkillKeyDown(KeyCode key)
    {
        if (selectedSkill != null) return;

        ActiveTypeSkills.TryGetValue(key, out selectedSkill);
        if (selectedSkill == null || selectedSkill.Stage != SkillStage.None)
        {
            ShowExecuteMsg("技能冷却中");
            selectedSkill = null;
            return;
        };

        //技能指示ui
        ShowSpellRangeUI(selectedSkill);

    }

    public void SkillKeyUp(KeyCode key)
    {
        if (selectedSkill == null) return;
        ActiveTypeSkills.TryGetValue(key, out var tmpSkill);
        if (selectedSkill != tmpSkill) return;

        SkillExecute(selectedSkill);
        selectedSkill = null;
    }


    /// <summary>
    /// 展示技能指示器
    /// </summary>
    public void ShowSpellRangeUI(Skill skill)
    {
        //技能圈圈
        ctrlController.unitUIController.SetSpellRangeCanvas(true, skill.Define.SpellRangeRadius * 0.001f);

        if (skill.IsUnitTarget)
        {
            //可以画一条从自己到target的直线



        }else if (skill.IsPointTarget)
        {
            //一个移动圆圈罢了
        }
        else
        {
            switch (skill.Define.EffectAreaType)
            {
                case "扇形":
                    ctrlController.unitUIController.SetSectorArea(true, skill.Define.SpellRangeRadius * 0.001f, 0f);
                    break;
                case "圆形":

                    break;
                case "矩形":

                    break;
            }
        }

    }

    /// <summary>
    /// 取消技能输入，关闭技能指示器
    /// </summary>
    public void CloseSpellRangeUI()
    {
        selectedSkill = null;
        ctrlController.unitUIController.SetSpellRangeCanvas(false);
        ctrlController.unitUIController.SetSectorArea(false);

    }

    /// <summary>
    /// 技能释放
    /// </summary>
    /// <param name="skill"></param>
    public void SkillExecute(Skill skill)
    {

        //关掉技能指示器
        ctrlController.unitUIController.SetSpellRangeCanvas(false);

        if (skill.IsUnitTarget)
        {
            //可以画一条从自己到target的直线

            if(_currentEnemy == null)
            {
                ShowExecuteMsg("需要指定目标");
                return;
            }

        }
        else if (skill.IsPointTarget)
        {
            //一个移动圆圈罢了
        }
        else
        {
            switch (skill.Define.EffectAreaType)
            {
                case "扇形":
                    //拿最后指向的方向
                    var dir = ctrlController.unitUIController.GetSectorAreaDir();

                    //设置到角色身上
                    if (dir != Quaternion.identity)
                    {
                        transform.rotation = dir;
                    }

                    //指示器关闭
                    ctrlController.unitUIController.SetSectorArea(false);

                    break;
                case "圆形":




                    break;
                case "矩形":



                    break;
            }
        }

        //向服务器发请求
        CombatService.Instance.SpellSkill(skill, _currentEnemy);
    }
    private void ShowExecuteMsg(string context)
    {
        UIManager.Instance.MessagePanel.ShowBottonMsg(context);
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
        Vector3 targetDirection = (target.renderObj.transform.position - transform.position).normalized;

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

    /// <summary>
    /// 移动到目标附近，直到复合攻击范围*0.8
    /// </summary>
    public void MoveToActorUntilCloseAttackRange()
    {




    }

}
