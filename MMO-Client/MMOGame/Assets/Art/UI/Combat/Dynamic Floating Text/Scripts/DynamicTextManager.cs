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


    //画布对象
    public static GameObject canvasPrefab;
    //主相机（飘字组件其他地方有引用）
    public static Transform mainCamera => Camera.main.transform;

    [SerializeField] private DynamicTextData _defaultData;
    [SerializeField] private DynamicTextData _critData;
    [SerializeField] private DynamicTextData _missData;
    [SerializeField] private GameObject _canvasPrefab;


    private void Awake()
    {
        defaultData = _defaultData;
        canvasPrefab = _canvasPrefab;
        critData = _critData;
    }

    public static void CreateText2D(Vector2 position, string text, DynamicTextData data)
    {
        GameObject newText = Instantiate(canvasPrefab, position, Quaternion.identity);
        newText.transform.GetComponent<DynamicText2D>().Initialise(text, data);
    }

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
