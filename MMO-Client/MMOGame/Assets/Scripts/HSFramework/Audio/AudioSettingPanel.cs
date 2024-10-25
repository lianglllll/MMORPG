using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingPanel : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button btnSure;
    
    void Start()
    {
        //AudioManager.Instance.PlayAudio(AudioDefine.Nostalgia_Loop);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            PlayBubble();
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(_bgmAudioSource)
            {
                AudioManager.Instance.RecycleAudio(_bgmAudioSource);
                _bgmAudioSource = null;
            }
        }
    }

    AudioSource _bgmAudioSource;
    private void PlayBubble()
    {
        //_bgmAudioSource = AudioManager.Instance.PlayAudio(AudioDefine.water);
    }

    private void OnMusicVolumeChanged(float arg0)
    {
        AudioManager.Instance.SetMusicVolume(arg0);
    }
    
    private void OnSFXVolumeChanged(float arg0)
    {
        AudioManager.Instance.SetSFXVolume(arg0);
    }
}
