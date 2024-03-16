using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberInputBox : MonoBehaviour
{
    private Text titleText;
    private Button affirmBtn;
    private Button cancelBtn;
    private Button addBtn;
    private Button subBtn;
    private InputField inputField;

    private int currentCount;               //记录当前文本框中的记录的数量
    private Action<int> _okAction;
    private int MaxCount;                   //最大的count

    private void Awake()
    {
        titleText = transform.Find("Title").GetComponent<Text>();
        affirmBtn = transform.Find("AffirmBtn").GetComponent<Button>();
        cancelBtn = transform.Find("CancelBtn").GetComponent<Button>();
        addBtn = transform.Find("AddBtn").GetComponent<Button>();
        subBtn = transform.Find("SubBtn").GetComponent<Button>();
        inputField = transform.Find("InputField").GetComponent<InputField>();   
    }

    void Start()
    {
        affirmBtn.onClick.AddListener(OnAffirmBtn);
        cancelBtn.onClick.AddListener(OnCancelBtn);
        addBtn.onClick.AddListener(OnAddBtn);
        subBtn.onClick.AddListener(OnSubBtn);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }


    public void Show(Vector3 pos, string title,int limitCount, Action<int> ok)
    {
        //位置
        transform.position = pos;

        //初始化为1
        currentCount = 1;
        MaxCount = limitCount;
        UpdateInputFieldText();
        _okAction = ok;

        //设置title
        UpdateTitleText(title);

        //置于顶层显示
        transform.SetAsFirstSibling();
        this.gameObject.SetActive(true);
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
        _okAction = null;
    }

    /// <summary>
    /// 更新输入框中的数字
    /// </summary>
    /// <param name="count"></param>
    public void UpdateInputFieldText()
    {
        inputField.text = "" + currentCount;
    }

    public void UpdateTitleText(string str)
    {
        titleText.text = "丢弃物品:" + str;
    }

    private void OnAffirmBtn()
    {
        _okAction?.Invoke(currentCount);
        Hide();
    }

    private void OnCancelBtn()
    {
        Hide();
    }

    private void OnAddBtn()
    {
        ++currentCount;
        currentCount = Math.Min(currentCount, MaxCount);
        UpdateInputFieldText();
    }

    private void OnSubBtn()
    {
        --currentCount;
        if (currentCount < 0) {
            currentCount = 0;
        }
        UpdateInputFieldText();
    }
}
