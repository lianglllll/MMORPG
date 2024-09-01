using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    private TextMeshPro SimpleTipsText;
    private TextMeshPro DetailTipsText;
    private Button ComfirmBtn;
    private Button CancelBtn;
    private Action comfirmAction;

    private void Awake()
    {
        SimpleTipsText = transform.Find("TipsBox/SimpleTipsText").GetComponent<TextMeshPro>();
        DetailTipsText = transform.Find("TipsBox/DetailTipsText").GetComponent<TextMeshPro>();
        ComfirmBtn = transform.Find("TipsBox/ComfirmBtn").GetComponent<Button>();
        CancelBtn = transform.Find("TipsBox/CancelBtn").GetComponent<Button>();
    }

    private void Start()
    {
        ComfirmBtn.onClick.AddListener(OnComfirm);
        CancelBtn.onClick.AddListener(OnCancel);
    }

    public void OpenPanel(string simpleTipsText,string detailTipsText, Action comfirmAction)
    {
        SimpleTipsText.text = simpleTipsText;
        DetailTipsText.text = detailTipsText;
        this.comfirmAction = comfirmAction;
    }

    public void OnComfirm()
    {
        comfirmAction?.Invoke();
    }


    public void OnCancel()
    {
        comfirmAction = null;
        gameObject.SetActive(true);
    }

}
