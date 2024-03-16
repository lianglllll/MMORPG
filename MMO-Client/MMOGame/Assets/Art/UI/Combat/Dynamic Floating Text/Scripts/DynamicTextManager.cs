using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicTextManager : MonoBehaviour
{
    //默认飘字效果
    public static DynamicTextData defaultData;
    //暴击的飘字效果
    public static DynamicTextData critData;
    //闪避的飘字效果
    public static DynamicTextData missData;
    //物理伤害
    public static DynamicTextData Physical;
    //法术伤害
    public static DynamicTextData Spell;
    //真实伤害
    public static DynamicTextData Real;


    //画布对象
    public static GameObject canvasPrefab;
    //主相机（飘字组件其他地方有引用）
    public static Transform mainCamera => Camera.main.transform;

    [SerializeField] private DynamicTextData _defaultData;
    [SerializeField] private DynamicTextData _critData;
    [SerializeField] private DynamicTextData _missData;
    [SerializeField] private DynamicTextData _physical;
    [SerializeField] private DynamicTextData _spell;
    [SerializeField] private DynamicTextData _real;
    [SerializeField] private GameObject _canvasPrefab;


    private void Awake()
    {
        defaultData = _defaultData;
        critData = _critData;
        missData = _missData;
        Physical = _physical;
        Spell = _spell;
        Real = _real;



        canvasPrefab = _canvasPrefab;




    }

    public static void CreateText2D(Vector2 position, string text, DynamicTextData data)
    {
        GameObject newText = Instantiate(canvasPrefab, position, Quaternion.identity);
        newText.transform.GetComponent<DynamicText2D>().Initialise(text, data);
    }


    /// <summary>
    /// 创建一个伤害飘字
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="data"></param>
    public static void CreateText(Vector3 position, string text, DynamicTextData data = null)
    {
        if (data == null)
        {
            data = defaultData;
        }
        GameObject newText = Instantiate(canvasPrefab, position, Quaternion.identity);
        newText.transform.GetComponent<DynamicText>().Initialise(text, data);
    }

}
