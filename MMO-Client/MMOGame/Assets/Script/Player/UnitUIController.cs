using GameClient.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIController : MonoBehaviour
{
    private Actor owner;

    private Canvas SelectMarkCanvas;        //锁定目标的ui

    private Canvas SpellRangeCanvas;        //攻击距离的ui
    private Image SpellRangeImage;




    private void Awake()
    {
        SelectMarkCanvas = transform.Find("MyCanvas/SelectMarkCanvas").GetComponent<Canvas>();

        SpellRangeCanvas = transform.Find("MyCanvas/SpellRangeCanvas").GetComponent<Canvas>();
        SpellRangeImage = SpellRangeCanvas.GetComponentInChildren<Image>();

    }

    private void Start()
    {
        SetSelectMark(false);
        SetSpellRangeCanvas(false);
    }

    public void Init(Actor actor)
    {
        this.owner = actor;
    }

    /// <summary>
    /// 设置选中标志是否显示
    /// </summary>
    public void SetSelectMark(bool enable)
    {
        SelectMarkCanvas.enabled = enable;
    }

    /// <summary>
    /// 设置攻击距离的ui
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="range"></param>
    public void SetSpellRangeCanvas(bool enable,float range = 3.0f)
    {
        if (enable)
        {
            SpellRangeImage.transform.localScale = Vector3.one*range;
            SpellRangeCanvas.enabled = enable;
        }
        else
        {
            SpellRangeCanvas.enabled = enable;
        }
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




}
