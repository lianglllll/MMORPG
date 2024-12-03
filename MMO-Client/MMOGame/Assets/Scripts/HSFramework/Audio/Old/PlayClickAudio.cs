using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayClickAudio : MonoBehaviour
{
    private void Awake()
    {
        var btn = gameObject.GetComponent<Button>();
        if (btn)
        {
            btn.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        AudioManager.Instance.PlayAudio(AudioDefine.click1,transform);
    }
}
