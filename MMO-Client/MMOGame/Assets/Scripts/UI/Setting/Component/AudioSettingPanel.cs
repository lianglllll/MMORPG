using HSFramework.Audio;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingPanel : BaseSettingPanel
{
    private Slider masterVolumeSlider;
    private Slider bgVolumeSlider;
    private Slider uiVolumeSlider;

    protected override void Awake()
    {
        base.Awake();
        masterVolumeSlider = transform.Find("MasterVolumeSettingBar/Slider").GetComponent<Slider>();
        bgVolumeSlider = transform.Find("BgVolumeSettingBar/Slider").GetComponent<Slider>();
        uiVolumeSlider = transform.Find("UIVolumeSettingBar/Slider").GetComponent<Slider>();
    }

    private void OnEnable()
    {
        // Initialize sliders with current volume levels
        masterVolumeSlider.value = Mathf.Pow(10, GlobalAudioManager.Instance.MasterVolume / 20) ;
        bgVolumeSlider.value = Mathf.Pow(10, GlobalAudioManager.Instance.BGMVolume / 20) ;
        uiVolumeSlider.value = Mathf.Pow(10, GlobalAudioManager.Instance.UIVolume / 20) ;

        // Add listeners
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgVolumeSlider.onValueChanged.AddListener(SetBGVolume);
        uiVolumeSlider.onValueChanged.AddListener(SetUIVolume);

        isChangeSetting = false;
    }

    private void OnDisable()
    {
        // Remove listeners to avoid memory leaks
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        bgVolumeSlider.onValueChanged.RemoveListener(SetBGVolume);
        uiVolumeSlider.onValueChanged.RemoveListener(SetUIVolume);
    }

    // Methods to update the audio manager's settings
    private void SetMasterVolume(float value)
    {
        GlobalAudioManager.Instance.MasterVolume = value; 
    }
    private void SetBGVolume(float value)
    {
        GlobalAudioManager.Instance.BGMVolume = value; 
    }
    private void SetUIVolume(float value)
    {
        GlobalAudioManager.Instance.UIVolume = value; 
    }

    protected override void OnReset()
    {
        base.OnReset();
        LocalDataManager.Instance.gameSettings.audioSetting.Reset();
    }
    protected override void OnSave()
    {
        base.OnSave();
        var audioSetting = LocalDataManager.Instance.gameSettings.audioSetting;
        audioSetting.masterVolume = masterVolumeSlider.value;
        audioSetting.bgVolume = bgVolumeSlider.value;
        audioSetting.uiVolume = uiVolumeSlider.value;
        LocalDataManager.Instance.SaveSettings();
    }

}
