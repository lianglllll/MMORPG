using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class ResolutionOption
{
    public int width;
    public int height;
}

public class VideoSettingPanel : MonoBehaviour
{
    [SerializeField]
    private List<ResolutionOption> m_resolutions;
    private TMP_Dropdown m_masterVolumeSettingDropdown;

    private void Awake()
    {
        m_masterVolumeSettingDropdown = transform.Find("MasterVolumeSettingBar/Dropdown").GetComponent<TMP_Dropdown>();
    }
    private void Start()
    {
        // 清空当前选项
        m_masterVolumeSettingDropdown.ClearOptions();

        // 创建一个列表来存储分辨率选项的字符串
        var options = new List<string>();
        options.Add("Full");
        foreach (var resolution in m_resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
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

    private void OnResolutionChange(int value)
    {
        if(value == 0)
        {
            Screen.fullScreen = true;
            // 全屏
            return;
        }
        // 设置屏幕分辨率
        ResolutionOption selectedResolution = m_resolutions[value - 1];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, false);
    }
}
