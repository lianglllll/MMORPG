using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    private TMP_InputField inputField;
    private TextMeshProUGUI SimpleTipsText;
    private TextMeshProUGUI DetailTipsText;
    private Button ComfirmBtn;
    private Button CancelBtn;
    private Action m_comfirmAction;
    private Action<string> m_comfirmAction2;
    private Action cancelAction;
    private bool m_isInput;

    private void Awake()
    {
        inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
        SimpleTipsText = transform.Find("TipsBox/SimpleTipsText").GetComponent<TextMeshProUGUI>();
        DetailTipsText = transform.Find("TipsBox/DetailTipsText").GetComponent<TextMeshProUGUI>();
        ComfirmBtn = transform.Find("TipsBox/ComfirmBtn").GetComponent<Button>();
        CancelBtn = transform.Find("TipsBox/CancelBtn").GetComponent<Button>();
        ComfirmBtn.onClick.AddListener(OnComfirm);
        CancelBtn.onClick.AddListener(OnCancel);
        inputField.gameObject.SetActive(false);
    }

    public void Init(Action action)
    {
        cancelAction = action;
        m_isInput = false;
    }

    public void OpenPanel(string simpleTipsText,string detailTipsText, Action comfirmAction)
    {
        SimpleTipsText.text = simpleTipsText;
        DetailTipsText.text = detailTipsText;
        m_comfirmAction = comfirmAction;
    }
    public void OpenPanelWithInput(string simpleTipsText, string detailTipsText, Action<string> comfirmAction)
    {
        SimpleTipsText.text = simpleTipsText;
        DetailTipsText.text = detailTipsText;
        m_comfirmAction2 = comfirmAction;
        m_isInput = true;
        inputField.gameObject.SetActive(true);
        inputField.gameObject.SetActive(true);
    }

    public void OnComfirm()
    {
        if (!m_isInput)
        {
            m_comfirmAction?.Invoke();
            m_comfirmAction = null;
        }
        else
        {
            m_comfirmAction2?.Invoke(inputField.text);
            m_comfirmAction2 = null;
            inputField.gameObject.SetActive(false);
        }
        OnCancel();
    }
    public void OnCancel()
    {
        cancelAction?.Invoke();
    }
}
