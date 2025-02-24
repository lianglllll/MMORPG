using System.Collections.Generic;
using TMPro;
using UnityEngine;


[System.Serializable]
public class ResolutionOption
{
    public int width;
    public int height;
}

public class VideoSettingPanel : BaseSettingPanel
{
    [SerializeField]
    private List<ResolutionOption> m_resolutions;
    private TMP_Dropdown m_masterVolumeSettingDropdown;

    private int CurIdx;
    private int targetIdx;

    protected override void Awake()
    {
        base.Awake();
        m_masterVolumeSettingDropdown = transform.Find("MasterVolumeSettingBar/Dropdown").GetComponent<TMP_Dropdown>();
    }
    protected override void Start()
    {
        base.Start();
        // 清空当前选项
        m_masterVolumeSettingDropdown.ClearOptions();

        // 创建一个列表来存储分辨率选项的字符串
        var options = new List<string>();
        options.Add("Full");
        for(int i = 1; i < m_resolutions.Count; i++)
        {
            string option = m_resolutions[i].width + " x " + m_resolutions[i].height;
            options.Add(option);
        }

        // 向 Dropdown 添加选项
        m_masterVolumeSettingDropdown.AddOptions(options);

        // 添加回调事件监听器
        m_masterVolumeSettingDropdown.onValueChanged.AddListener(OnResolutionChange);
    }
    private void OnDestroy()
    {
        // 移除事件监听器（防止内存泄漏）
        m_masterVolumeSettingDropdown.onValueChanged.RemoveListener(OnResolutionChange);
    }
    private void OnEnable()
    {
        isChangeSetting = false;
        var viodeSetting = LocalDataManager.Instance.gameSettings.videoSetting;
        CurIdx = viodeSetting.resolutionIdx;
        targetIdx = viodeSetting.resolutionIdx;
        m_masterVolumeSettingDropdown.value = CurIdx;
    }

    protected override void OnReset()
    {
        base.OnReset();
        var viodeSetting = LocalDataManager.Instance.gameSettings.videoSetting;
        viodeSetting.Reset();
        CurIdx = viodeSetting.resolutionIdx;
        targetIdx = viodeSetting.resolutionIdx;
        m_masterVolumeSettingDropdown.value = CurIdx;
        // 
        ResolutionOption selectedResolution = m_resolutions[CurIdx];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, false);
    }
    protected override void OnSave()
    {
        base.OnSave();
        if (targetIdx == CurIdx)
        {
            return;
        }
        CurIdx = targetIdx;
        bool isFull = false;
        if(CurIdx == 0)
        {
            isFull = true;
        }
        ResolutionOption selectedResolution = m_resolutions[CurIdx];
        var viodeSetting = LocalDataManager.Instance.gameSettings.videoSetting;
        viodeSetting.resolutionWidth = selectedResolution.width;
        viodeSetting.resolutionHeight = selectedResolution.height;
        viodeSetting.resolutionIdx = CurIdx;
        viodeSetting.isFull = isFull;
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, false);
        LocalDataManager.Instance.SaveSettings();
    }
    private void OnResolutionChange(int value)
    {
        if(value == CurIdx)
        {
            return;
        }
        targetIdx = value;
    }
}
