using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePanel : MonoBehaviour
{
    private TextMeshProUGUI SimpleTipsText;
    private TextMeshProUGUI DetailTipsText;
    private Button ComfirmBtn;
    private Button CancelBtn;
    private Action comfirmAction;
    private Action cancelAction;

    private void Awake()
    {
        SimpleTipsText = transform.Find("TipsBox/SimpleTipsText").GetComponent<TextMeshProUGUI>();
        DetailTipsText = transform.Find("TipsBox/DetailTipsText").GetComponent<TextMeshProUGUI>();
        ComfirmBtn = transform.Find("TipsBox/ComfirmBtn").GetComponent<Button>();
        CancelBtn = transform.Find("TipsBox/CancelBtn").GetComponent<Button>();
    }

    private void Start()
    {
        ComfirmBtn.onClick.AddListener(OnComfirm);
        CancelBtn.onClick.AddListener(OnCancel);
    }

    public void OpenPanel(string simpleTipsText,string detailTipsText, Action comfirmAction,Action cancelAction)
    {
        SimpleTipsText.text = simpleTipsText;
        DetailTipsText.text = detailTipsText;
        this.comfirmAction = comfirmAction;
        this.cancelAction = cancelAction;
    }

    public void OnComfirm()
    {
        comfirmAction?.Invoke();
        gameObject.SetActive(false);
        cancelAction = null;
        comfirmAction = null;
    }


    public void OnCancel()
    {
        cancelAction?.Invoke();
        gameObject.SetActive(false);
        cancelAction = null;
        comfirmAction = null;
    }

}
