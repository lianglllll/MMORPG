using HSFramework.Audio;
using UnityEngine;
using UnityEngine.UI;

public class BaseSettingPanel : MonoBehaviour
{
    protected bool isChangeSetting;
    protected Button resetBtn;
    protected Button saveBtn;

    protected virtual void Awake()
    {
        resetBtn = transform.Find("ReSetBtn").GetComponent<Button>();
        saveBtn = transform.Find("SaveBtn").GetComponent<Button>();
    }

    protected virtual void Start()
    {
        resetBtn.onClick.AddListener(OnReset);
        saveBtn.onClick.AddListener(OnSave);
    }

    protected virtual void OnReset() {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
    }
    protected virtual void OnSave() {
        GlobalAudioManager.Instance.PlayUIAudio(UIAudioClipType.ButtonClick);
    }

}
