using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 通用的确认面板
/// 传送、复活、断线重连
/// </summary>
public class ConfirmBox : MonoBehaviour
{
    public int spaceId;
    private Action onClickBtnDefaultCallback;
    private Action _onClickBtnCallback;

    private Button confirmBtn;
    private Text confirmBtnText;
    private Text tipsText;


    private void Awake()
    {
        confirmBtn = transform.Find("ConfirmBtn").GetComponent<Button>();
        confirmBtnText = transform.Find("ConfirmBtn/Text").GetComponent<Text>();
        tipsText = transform.Find("TipsBox/Text").GetComponent<Text>();
    }

    private void Start()
    {
        confirmBtn.onClick.AddListener(onClick);
        spaceId = -1;
    }

    public void Init(Action action)
    {
        onClickBtnDefaultCallback = action;
    }

    private void onClick()
    {
        _onClickBtnCallback?.Invoke();
        onClickBtnDefaultCallback?.Invoke();//通知某某关掉本面板
    }

    public void ShowBox(string tipsContent,string btnContent, bool btnActive,Action onClickBtnCallback)
    {
        confirmBtn.enabled = btnActive;
        this.tipsText.text = tipsContent;
        this.confirmBtnText.text = btnContent;
        _onClickBtnCallback = onClickBtnCallback;
    }

    public void CloseBox()
    {
        _onClickBtnCallback = null;
    }

}
