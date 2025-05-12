using GameClient;
using GameClient.Combat;
using GameClient.Entities;
using GameClient.Manager;
using HS.Protobuf.SceneEntity;
using Player;
using Player.Controller;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocalComboData
{
    public Skill skill;
    public LocalComboData next;

    public LocalComboData()
    {
        skill = null;
        next = null;
    }

    public LocalComboData(Skill skill)
    {
        this.skill = skill;
        next = null;
    }
}


/// <summary>
/// 角色的战斗控制器
/// 现在主要是用来控制什么时候使用skill
/// </summary>
public class LocalPlayerCombatController : MonoBehaviour
{
    private bool m_IsStart;

    private LocalPlayerController m_localPlayerController;
    private SkillManager m_skillManager;

    // 普通攻击连招表(全部连招表第一招都是空的，因为在攻击输入的时候会自动读取下一招)
    // 普通攻击全是有目标技能
    private Dictionary<NetActorMode, LocalComboData> m_defaultComboDataDict;
    private LocalComboData m_currentComboData;
    private bool m_isComboBufferTime;
    private float m_remainComboBufferTime;
    private float m_comboBufferInterval = 2f;
    // private int sendButDotRecive = 0;// TODO 当玩家攻击按得比较快的时候可能会有问题。。

    // 主动技能字典
    private Dictionary<int, Skill> ActiveTypeSkills;
    protected Skill selectedSkill;                                              // 当前输入的主动技能

    // 敌人的范围检测
    private Actor m_owner => m_localPlayerController.Actor;
    private Actor m_curLockTarget;
    protected float m_detectionRange = 15f;
    private LayerMask m_enemyLayer;

    private void Start()
    {
        m_enemyLayer = LayerMask.GetMask("Actor");
    }
    private void Update()
    {
        if (!m_IsStart) return;

        // todo 装武器
        if (GameInputManager.Instance.Equip)
        {
            if(m_localPlayerController.CurMode == NetActorMode.Normal)
            {
                m_localPlayerController.ChangeMode(NetActorMode.NormalEquip);
            }
            else if(m_localPlayerController.CurMode == NetActorMode.NormalEquip)
            {
                m_localPlayerController.ChangeMode(NetActorMode.Normal);
            }
            else if(m_localPlayerController.CurMode == NetActorMode.Fly)
            {
                m_localPlayerController.ChangeMode(NetActorMode.FlyEquip);
            }
            else if(m_localPlayerController.CurMode == NetActorMode.FlyEquip)
            {
                m_localPlayerController.ChangeMode(NetActorMode.Fly);
            }
        }

        // SelectTargetObject();
        LockTargetWithRecently();
        PlayerAttackInput();
        ComputeComboTimeOut();
        ClearLockTargetWhenMotion();
    }
    private void OnEnable()
    {
        // new
        Kaiyun.Event.RegisterIn("LocalPlayerModeChange", this, "ChangeOrResetCurCombo");
        Kaiyun.Event.RegisterIn("SkillActiveEnd", this, "SkillActiveEnd");

        // 老旧 目标死亡
        Kaiyun.Event.RegisterIn("TargetDeath", this, "ClearEnemy");
        Kaiyun.Event.RegisterOut("CtlChrDeath", this, "ClearEnemy");
    }
    private void OnDisable()
    {
        Kaiyun.Event.UnRegisterIn("LocalPlayerModeChange", this, "ChangeOrResetCurCombo");

        Kaiyun.Event.UnRegisterIn("TargetDeath", this, "ClearEnemy");
        Kaiyun.Event.UnRegisterOut("CtlChrDeath", this, "ClearEnemy");
        Kaiyun.Event.UnRegisterIn("SkillActiveEnd", this, "SkillActiveEnd");
    }
    public void Init(LocalPlayerController ctrlController)
    {
        m_localPlayerController = ctrlController;
        m_skillManager = ctrlController.Actor.m_skillManager;

        // 初始化普通攻击连招表
        m_defaultComboDataDict = new();
        WeaponSkillArsenalDefine nDef = LocalDataManager.Instance.
            m_weaponSkillArsenalDefineDict[ctrlController.Actor.UnitDefine.weaponSkillArsenalId];
        LocalComboData nCb = new LocalComboData();
        m_defaultComboDataDict.Add(NetActorMode.Normal, nCb);
        LocalComboData lastComboData = nCb;
        foreach (int skillId in nDef.SkillIds)
        {
            var skill = m_skillManager.GetSkillBySkillId(skillId);
            var tmpComboData = new LocalComboData(skill);
            lastComboData.next = tmpComboData;
            lastComboData = tmpComboData;
        }
        lastComboData.next = nCb.next;
        m_currentComboData = nCb;

        // TODO 根据武器类型装载,这里只是模拟的数据，因为当前装备没有做好
        m_skillManager.AddSkillArsenal(1);
        LocalComboData nECb = new LocalComboData();
        m_defaultComboDataDict.Add(NetActorMode.NormalEquip, nECb);
        WeaponSkillArsenalDefine nEDef = LocalDataManager.Instance.m_weaponSkillArsenalDefineDict[1];
        lastComboData = nECb;
        foreach (int skillId in nEDef.SkillIds)
        {
            var skill = m_skillManager.GetSkillBySkillId(skillId);
            var tmpComboData = new LocalComboData(skill);
            lastComboData.next = tmpComboData;
            lastComboData = tmpComboData;
        }
        lastComboData.next = nECb.next;

        // 初始化主动技能
        ActiveTypeSkills = m_skillManager.GetFixedSkills();

        m_IsStart = true;
    }

    private void PlayerAttackInput()
    {
        if (!CanAttackInput())
        {
            return;
        }

        // 鼠标左键的base攻击
        if (GameInputManager.Instance.LAttackPressed)            
        {
            // 施法范围圈
            m_localPlayerController.unitUIController.SetSpellRangeCanvas(true, m_currentComboData.next.skill.Define.SpellRangeRadius * 0.001f);
        }
        if (GameInputManager.Instance.LAttackReleased)
        {
            SetStandComboData(m_currentComboData.next);
            BaseComboActionExecute();
            // 关闭攻击范围
            m_localPlayerController.unitUIController.SetSpellRangeCanvas(false);
        }

        // 固定技能攻击
        SkillInput();
    }
    private bool CanAttackInput()
    {
        if (m_localPlayerController.CurSkill != null && !m_localPlayerController.IsCanSwitchSkill) return false;
        if (m_currentComboData == null) return false;
        if (m_localPlayerController.CurState == NetActorState.Hurt) return false;
        // if (m_localPlayerController.CurState == NetActorState.Skill) return false;
        if (m_localPlayerController.CurState == NetActorState.Dizzy) return false;
        if (m_localPlayerController.CurState == NetActorState.Death) return false;
        if (m_localPlayerController.CurState == NetActorState.Defense) return false;
        return true;
    }

    // 基础攻击连招
    private void SetStandComboData(LocalComboData next)
    {
        if (next == null) return;
        m_currentComboData = next;
    }
    private void BaseComboActionExecute()
    {
        /// 执行普通攻击,发请求告知服务器，因为普通攻击的特殊性有下面的要求：
        /// 1.当有锁定敌人的时候，我们需要面向敌人
        /// 2.当没有锁定敌人的时候，不做其他处理，也就是原地放技能

        if ( m_curLockTarget != null)
        {
            LookAtTarget(m_curLockTarget);
            // 已经锁定了一个最近的目标，计算当前的攻击距离
            if (Vector3.Distance(transform.position, m_curLockTarget.RenderObj.transform.position) > m_currentComboData.skill.Define.SpellRangeRadius * 0.001f)
            {
                // 打不到敌人，自己原地平啊
                // 获取移动到可以攻击到敌人的位置
            }
            else
            {
                // 剩下就是打敌人的了
            }
        }
        _SpellSkill(m_currentComboData.skill,m_curLockTarget);
    End:
        return;
    }
    public void SkillActiveEnd()
    {
        /// 攻击动作结束后，招式重置为第一招
        /// 基础连招->重置为基础连招
        /// 技能->重置为基础连招

        //开启连招缓存时间
        m_remainComboBufferTime = m_comboBufferInterval;
        m_isComboBufferTime = true;
    }
    private void ComputeComboTimeOut()
    {
        // 连招超时没有接上，重置combo的第一招
        if (m_isComboBufferTime)
        {
            m_remainComboBufferTime -= Time.deltaTime;
            if(m_remainComboBufferTime <= 0f)
            {
                ChangeOrResetCurCombo();
            }
        }
    }
    public void ChangeOrResetCurCombo()
    {
        if (!m_IsStart) return;
        if (!m_defaultComboDataDict.TryGetValue(m_localPlayerController.CurMode, out var cb))
        {
            m_currentComboData = null;
            goto End;
        }
        m_currentComboData = cb;
        m_isComboBufferTime = false;
    End:
        return;
    }

    // 技能
    public void SkillInput()
    {
        // 按下
        if (selectedSkill == null)
        {
            int key = 0;
            if (GameInputManager.Instance.KeyOneDown)
            {
                key = 1;
            }else if (GameInputManager.Instance.KeyTwoDown)
            {
                key = 2;
            }
            else if (GameInputManager.Instance.KeyThreeDown)
            {
                key = 3;
            }
            else if (GameInputManager.Instance.KeyFourDown)
            {
                key = 4;
            }
            if(key != 0)
            {
                SkillKeyDown(key);
            }

        }

        // 抬起
        if (selectedSkill != null)
        {
            int key = 0;
            if (GameInputManager.Instance.KeyOneReleased)
            {
                key = 1;
            }
            else if (GameInputManager.Instance.KeyTwoReleased)
            {
                key = 2;
            }
            else if (GameInputManager.Instance.KeyThreeReleased)
            {
                key = 3;
            }
            else if (GameInputManager.Instance.KeyFourReleased)
            {
                key = 4;
            }
            if (key != 0)
            {
                SkillKeyUp(key);
            }
        }

        // 取消技能释放
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CloseSpellRangeUI();
        }

    }
    public void SkillKeyDown(int key)
    {
        ActiveTypeSkills.TryGetValue(key, out selectedSkill);
        if(selectedSkill == null)
        {
            goto End;
        }
        else if (selectedSkill.Stage != SkillStage.None)
        {
            ShowExecuteMsg("技能冷却中");
            selectedSkill = null;
            goto End;
        }

        // 技能指示ui
        ShowSpellRangeUI(selectedSkill);

    End:
        return;
    }
    public void SkillKeyUp(int key)
    {
        ActiveTypeSkills.TryGetValue(key, out var tmpSkill);
        if (selectedSkill != tmpSkill)
        {
            goto End;
        }
        SkillExecute(selectedSkill);
        selectedSkill = null;
    End:
        return;
    }
    public void SkillExecute(Skill skill)
    {
        //关掉技能指示器
        m_localPlayerController.unitUIController.SetSpellRangeCanvas(false);

        if (skill.IsUnitTarget)
        {
            //可以画一条从自己到target的直线

            if (m_curLockTarget == null)
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
                    var dir = m_localPlayerController.unitUIController.GetSectorAreaDir();

                    //设置到角色身上
                    if (dir != Quaternion.identity)
                    {
                        transform.rotation = dir;
                    }

                    //指示器关闭
                    m_localPlayerController.unitUIController.SetSectorArea(false);

                    break;
                case "圆形":


                    break;
                case "矩形":

                    break;
            }
        }

        //向服务器发请求
        _SpellSkill(skill, m_curLockTarget);

        // 将我们的普通攻击重置为第一招
        ChangeOrResetCurCombo();
    }
    private void ShowExecuteMsg(string context)
    {
        UIManager.Instance.MessagePanel.ShowBottonMsg(context);
    }

    // 目标锁定
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
                    LockTargetWithManual(clickedObject.GetComponent<BaseController>().Actor);
                    Kaiyun.Event.FireOut("SelectTarget");
                }
                else if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    // localPlayerController.MoveToPostion(hitInfo.point);
                }
                else
                {

                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearLockTarget();
        }
    }
    protected int LockTargetWithRecently()
    {
        if (!GameInputManager.Instance.GI_Caps)
        {
            return -1;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return -1;
        Transform cameraTransform = mainCamera.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 cameraForward = cameraTransform.forward;

        // 通过碰撞器获取周围的敌人
        Collider[] enemys = Physics.OverlapSphere(transform.position + (transform.up) * 0.7f,
            m_detectionRange, m_enemyLayer, QueryTriggerInteraction.Ignore);// 没有collider的检测不到
        if (enemys == null || enemys.Length <= 1) return -1;

        Actor targetActor = null;
        float minDistance = float.MaxValue;
        Vector3 targetPos;
        Vector3 toTargetDir;
        for (int i = 0; i < enemys.Length; ++i)
        {
            var tmpA = enemys[i].GetComponent<BaseController>().Actor;
            if (m_owner == tmpA || tmpA.IsDeath) continue;

            // 摄像机到敌人的方向向量
            targetPos = tmpA.RenderObj.transform.position;
            toTargetDir = targetPos - cameraPosition;
            toTargetDir.Normalize();

            // 使用点积判断是否在摄像机前方（夹角小于90度）
            float dot = Vector3.Dot(cameraForward, toTargetDir);
            if (dot <= 0) continue; // 忽略背后和侧面的敌人

            // 计算与玩家的距离（可根据需求改为到摄像机的距离）
            var dis = Vector3.Distance(transform.position,tmpA.RenderObj.transform.position);
            if(dis < minDistance)
            {
                targetActor = tmpA;
                minDistance = dis;
            }
        }

        if (targetActor != null)
        {
            LockTargetWithManual(targetActor);
            return 0;
        }

        return -1;
    }
    public void LockTargetWithManual(Actor target)
    {
        if (target == null) return;
        ClearLockTarget();
        m_curLockTarget = target;
        GameApp.target = m_curLockTarget;
        m_curLockTarget.m_baseController.unitUIController.SetSelectedMark(true);
        Kaiyun.Event.FireOut("SelectTarget"); // ui
    }
    public void ClearLockTarget()
    {
        if (m_curLockTarget == null || GameApp.target == null) return;

        //触发一下取消敌人锁定的事件
        Kaiyun.Event.FireOut("CancelSelectTarget");//ui
        m_curLockTarget.m_baseController.unitUIController.SetSelectedMark(false);

        GameApp.target = null;
        m_curLockTarget = null;
    }
    private void ClearLockTargetWhenMotion()
    {
        if(m_curLockTarget !=null && m_localPlayerController.CurState == NetActorState.Motion && 
            Vector3.Distance(transform.position,m_curLockTarget.RenderObj.transform.position) > m_detectionRange)
        {
            ClearLockTarget();
        }
    }

    // Tools
    private void _SpellSkill(Skill skill, Actor target = null)
    {
        CombatHandler.Instance.SendSpellCastReq(skill, m_curLockTarget);
        Kaiyun.Event.FireIn("EnterCombatEvent");
        m_isComboBufferTime = false;
    }
    public void LookAtTarget(Actor target)
    {
        if (target == null) return;
        //transform.LookAt(target.renderObj.transform.position);

        // 计算角色应该朝向目标点的方向
        Vector3 targetDirection = (target.RenderObj.transform.position - transform.position).normalized;

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
    public void MoveToActorUntilCloseAttackRange()
    {
        // 移动到目标附近，直到在攻击范围 * 0.8 范围内
    }
    public void ShowSpellRangeUI(Skill skill)
    {
        //技能圈圈
        // m_localPlayerController.unitUIController.SetSpellRangeCanvas(true, skill.Define.SpellRangeRadius * 0.001f);

        if (skill.IsUnitTarget)
        {
            //可以画一条从自己到target的直线



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
                    m_localPlayerController.unitUIController.SetSectorArea(true, skill.Define.SpellRangeRadius * 0.001f, 0f);
                    break;
                case "圆形":

                    break;
                case "矩形":

                    break;
            }
        }

    }
    public void CloseSpellRangeUI()
    {
        selectedSkill = null;
        m_localPlayerController.unitUIController.SetSpellRangeCanvas(false);
        m_localPlayerController.unitUIController.SetSectorArea(false);

    }
}
