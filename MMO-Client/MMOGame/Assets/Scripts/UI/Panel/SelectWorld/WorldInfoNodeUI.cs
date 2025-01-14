using HS.Protobuf.Login;
using Serilog;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldInfoNodeUI : MonoBehaviour
{
    private WorldInfoNode m_worldInfoNode;
    private Image m_lockOulineImg;
    private Image m_stateIcon;
    private TextMeshProUGUI m_worldNameText;
    private Button selectBtn;

    private Action<WorldInfoNode> m_onSelectBtn;

    private void Awake()
    {
        m_lockOulineImg = transform.Find("LockOutline").GetComponent<Image>();
        m_stateIcon = transform.Find("StateIcon").GetComponent<Image>();
        m_worldNameText = transform.Find("WorldNameText").GetComponent<TextMeshProUGUI>();
        selectBtn = GetComponent<Button>();
    }
    private void Start()
    {
        selectBtn.onClick.AddListener(OnSelectBtn);
    }
    public bool Init(WorldInfoNode worldInfoNode, Action<WorldInfoNode> OnSelectBtn)
    {
        m_worldInfoNode = worldInfoNode;
        m_lockOulineImg.enabled = false;
        m_worldNameText.text = worldInfoNode.WorldName;

        string hexColor = "";
        if (worldInfoNode.Status == WORLD_LOAD_STATUS.Idle)
        {
            hexColor = "#3CCC1C";
        }
        else if (worldInfoNode.Status == WORLD_LOAD_STATUS.Offline)
        {
            hexColor = "#000000";
        }
        else if (worldInfoNode.Status == WORLD_LOAD_STATUS.Congested)
        {
            hexColor = "#FFCD39";
        }
        else if (worldInfoNode.Status == WORLD_LOAD_STATUS.Overloaded)
        {
            hexColor = "#FF1E00";
        }
        ColorUtility.TryParseHtmlString(hexColor, out Color newColor);
        newColor.a = 64 / 255f;
        m_stateIcon.color = newColor;

        m_onSelectBtn = OnSelectBtn;
        return true;
    }

    private void OnSelectBtn()
    {
        m_onSelectBtn?.Invoke(m_worldInfoNode);
    }

}


