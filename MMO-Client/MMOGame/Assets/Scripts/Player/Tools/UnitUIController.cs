using GameClient.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIController : MonoBehaviour
{

    private Actor owner;

    //actor的角色名和血条ui模块
    private UnitBillboard unitBillboard;


    //技能指示器模块
    //最后技能指示器拿到的就是一个dir or 坐标，用于技能的释放请求的参数。
    private Canvas SelectMarkCanvas;        //被锁定锁定目标的ui
    private Canvas SpellRangeCanvas;        //攻击距离的ui
    private Canvas SectorAreaCanvas;        //扇形区域
    private Image SpellRangeImage;
    private Image SectorAreaImage;

    private RaycastHit hit;                 //中间使用到的东西
    private Ray ray;
    private Vector3 curPos;
    private LayerMask groundLayer;
    private bool isUse;                     //是否被使用

    private void Awake()
    {
        SelectMarkCanvas = transform.Find("MyCanvas/SelectMarkCanvas").GetComponent<Canvas>();
        SpellRangeCanvas = transform.Find("MyCanvas/SpellRangeCanvas").GetComponent<Canvas>();
        SpellRangeImage = SpellRangeCanvas.GetComponentInChildren<Image>();
        SectorAreaCanvas = transform.Find("MyCanvas/SectorAreaCanvas").GetComponent<Canvas>();
        SectorAreaImage = SectorAreaCanvas.GetComponentInChildren<Image>();
        unitBillboard = transform.Find("MyCanvas/UnitBillboard").GetComponent<UnitBillboard>();
    }
    private void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        SelectMarkCanvas.gameObject.SetActive(true);
        SpellRangeCanvas.gameObject.SetActive(true);
        SectorAreaCanvas.gameObject.SetActive(true);

        SetSelectedMark(false);
        SetSpellRangeCanvas(false);
        SetSectorArea(false,0,0);
    }
    private void Update()
    {
        // 扇形ui被激活
        if (SectorAreaImage.enabled)
        {
            // 直接获取摄像机的Y轴旋转角度
            float targetYRotation = Camera.main.transform.eulerAngles.y;

            // 保留Canvas原有的X和Z轴旋转，仅同步Y轴
            Quaternion targetRotation = Quaternion.Euler(
                SectorAreaCanvas.transform.eulerAngles.x,
                targetYRotation,
                SectorAreaCanvas.transform.eulerAngles.z
            );

            // 应用旋转
            SectorAreaCanvas.transform.rotation = Quaternion.Lerp(
                SectorAreaCanvas.transform.rotation,
                targetRotation,
                Time.deltaTime * 20f  // 调整插值速度
            );

/*            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                curPos = new(hit.point.x, hit.point.y, hit.point.z);
            }

            // 让图标转向我们鼠标指向的地方
            Quaternion ab1canvas = Quaternion.LookRotation(curPos - transform.position);
            ab1canvas.eulerAngles = new Vector3(0, ab1canvas.eulerAngles.y, ab1canvas.eulerAngles.z);
            SectorAreaCanvas.transform.rotation = Quaternion.Lerp(ab1canvas, SectorAreaCanvas.transform.rotation, 0);*/
        }

        if(owner != null)
        {
            // ShowEntityInfoBar();
        }

    }
    public void Init(Actor actor)
    {
        this.owner = actor;
    }

    public void SetSelectedMark(bool enable)
    {
        SelectMarkCanvas.enabled = enable;
    }
    public void SetSpellRangeCanvas(bool enable,float range = 3.0f)
    {
        if (enable)
        {
            SpellRangeImage.transform.localScale = Vector3.one * range;
            SpellRangeCanvas.enabled = enable;
        }
        else
        {
            SpellRangeCanvas.enabled = enable;
        }
    }
    public void SetSectorArea(bool enable,float radius = 0f,float angle = 0f)
    {
        if (enable)
        {
            if (isUse) return;
            isUse = true;
            SectorAreaImage.enabled = enable;
            SectorAreaImage.transform.localScale = new Vector3(radius, radius, radius);
            SectorAreaImage.transform.localPosition = new Vector3(SectorAreaImage.transform.localPosition.x, SectorAreaImage.transform.localPosition.y, radius / 2);

            // 方向和摄像机对准
            // 直接获取摄像机的Y轴旋转角度
            float targetYRotation = Camera.main.transform.eulerAngles.y;

            // 保留Canvas原有的X和Z轴旋转，仅同步Y轴
            Vector3 newRotation = SectorAreaCanvas.transform.eulerAngles;
            newRotation.y = targetYRotation;

            // 直接应用旋转（无插值，立即同步）
            SectorAreaCanvas.transform.eulerAngles = newRotation;

        }
        else
        {
            isUse = false;
            SectorAreaImage.enabled = enable;
        }
    }
    public Quaternion  GetSectorAreaDir()
    {
        //扇形：dir
        //点：pos
        //圆：没得
        if (SectorAreaImage.enabled)
        {
            return SectorAreaCanvas.transform.rotation;
        }

        return Quaternion.identity;
    }

    #region 技能指示器ui（还没用上）
    /*
    //下面模拟一个技能指示器ui的逻辑
    private Canvas ability1;        //1.大指向技能
    private Image ability1Image;

    private Canvas ability2;        //2.点技能
    private float maxAbility2Distance;//点技能的最大距离
    private Canvas ability3;        //3.扇形技能就不写了，和大指向一模一样


    private RaycastHit hit;
    private Ray ray;
    private Vector3 curPos;
    private void ononUpdate()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Ability1Input();

        Ablity1Canvas();

    }
    //input
    private void Ability1Input()
    {
        //这里模拟要使用ability1
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //启用ability1的ui，禁用其他的ui
            ability1.enabled = true;
            ability1Image.enabled = true;

            ability2.enabled = false;
            //...
            //...

        }
    }
    private void Ablity1Canvas()
    {
        if (ability1.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                curPos = new(hit.point.x, hit.point.y, hit.point.z);
            }

            //让图标转向我们鼠标指向的地方
            Quaternion ab1canvas = Quaternion.LookRotation(curPos - transform.position);
            ab1canvas.eulerAngles = new Vector3(0, ab1canvas.eulerAngles.y, ab1canvas.eulerAngles.z);
            ability1.transform.rotation = Quaternion.Lerp(ab1canvas, ability1.transform.rotation, 0);
        }
    }


    private void Ablity2Canvas()
    {

        if (ability2.enabled)
        {
            int layerMask = ~LayerMask.GetMask("Player");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                curPos = new(hit.point.x, hit.point.y, hit.point.z);
            }

            //让图标显示到我们鼠标指向的地方
            var hitPosDir = (hit.point - transform.position).normalized;
            float distance = Vector3.Distance(hit.point, transform.position);
            distance = Mathf.Min(distance, maxAbility2Distance);
            var newHitPos = transform.position + hitPosDir * distance;
            ability2.transform.position = newHitPos;
        }
    }
    */
    #endregion


    /// <summary>
    /// 显示3维的角色名和血条，
    /// </summary>
    private void ShowEntityInfoBar()
    {
        // 面板看向摄像机的代码再Billboard中
        unitBillboard.slider.value = owner.Hp;
        unitBillboard.slider.maxValue = owner.MaxHp;
        unitBillboard.nameText.text = owner.ActorName;
        unitBillboard.gameObject.SetActive(owner.Hp > 0);
    }

}
