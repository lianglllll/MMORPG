using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
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

    public void Init(Action action)
    {
        cancelAction = action;
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
        OnCancel();
    }

    public void OnCancel()
    {
        comfirmAction = null;
        gameObject.SetActive(false);
        cancelAction?.Invoke();
    }

}
