using HSFramework.Setting;
using HSFramework.MySingleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace HSFramework.Audio
{
    public enum UIAudioClipType
    {
        ButtonClick
    }

    [Serializable]
    public class UIAudioClipEntry
    {
        public UIAudioClipType ClipType;
        public AudioClip Clip;
    }

    public class GlobalAudioManager : Singleton<GlobalAudioManager>
    {
        [SerializeField]
        private AudioMixer audioMixer;
        private AudioSource m_bGAudioSource;
        private AudioSource m_UIAudioSource;
        public List<AudioClip> bgAudioClips = new();
        public List<UIAudioClipEntry> UIAudioClips = new();
        private Dictionary<UIAudioClipType, AudioClip> m_uiAudioClipDictionary;
        private float minVolumValue = 0.0001f;
        private float fadeDuration = 1.0f; // 淡出时间（秒）
        private Coroutine fadeOutCoroutine = null; // 用于跟踪当前淡出的协程

        public float MasterVolume
        {
            get
            {
                audioMixer.GetFloat("MasterVolume", out float value); 
                return value;
            }
            set
            {
                float adjustedValue = Mathf.Max(value, minVolumValue);                  // 确保 value 不小于 minValue
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(adjustedValue) * 20);   // 转换为分贝
            }
        }
        public float BGMVolume
        {
            get
            {
                audioMixer.GetFloat("BGMVolume", out float value); // 转换为分贝
                return value;
            }
            set
            {
                float adjustedValue = Mathf.Max(value, minVolumValue);               // 确保 value 不小于 minValue
                audioMixer.SetFloat("BGMVolume", Mathf.Log10(adjustedValue) * 20);   // 转换为分贝
            }
        }
        public float UIVolume
        {
            get
            {
                audioMixer.GetFloat("UIVolume", out float value); // 转换为分贝
                return value;
            }
            set
            {
                float adjustedValue = Mathf.Max(value, minVolumValue);              // 确保 value 不小于 minValue
                audioMixer.SetFloat("UIVolume", Mathf.Log10(adjustedValue) * 20);   // 转换为分贝
            }
        }
        public float UnitVolume
        {
            get
            {
                audioMixer.GetFloat("UnitVolume", out float value); // 转换为分贝
                return value;
            }
            set
            {
                float adjustedValue = Mathf.Max(value, minVolumValue);              // 确保 value 不小于 minValue
                audioMixer.SetFloat("UnitVolume", Mathf.Log10(adjustedValue) * 20);   // 转换为分贝
            }
        }

        protected override void Awake()
        {
            base.Awake();
            m_bGAudioSource = transform.Find("BGAudioManager").GetComponent<AudioSource>();
            m_UIAudioSource = transform.Find("UIAudioManager").GetComponent<AudioSource>();
        }
        public void Init(AudioSettingData audioSetting)
        {
            m_uiAudioClipDictionary = new Dictionary<UIAudioClipType, AudioClip>();
            foreach (var entry in UIAudioClips)
            {
                if (!m_uiAudioClipDictionary.ContainsKey(entry.ClipType))
                {
                    m_uiAudioClipDictionary.Add(entry.ClipType, entry.Clip);
                }
            }

            // 音量设置
            MasterVolume = audioSetting.masterVolume;
            BGMVolume = audioSetting.bgVolume;
            UIVolume = audioSetting.uiVolume;
            UnitVolume = audioSetting.unitVolume;
        }

        public void PlayBackgroundAudio(string clipName)
        {
            //todo 用对象池
            AudioClip clip = Res.LoadAssetSync<AudioClip>(clipName);

            if (clip != null)
            {
                m_bGAudioSource.clip = clip;
                m_bGAudioSource.loop = true;
                m_bGAudioSource.Play();
            }
            else
            {
                Debug.LogError("Audio clip not found: " + clipName);
            }
        }
        public void StopBackgroundAudio()
        {
            // 如果当前已经有一个淡出协程在运行，则直接返回，避免重复操作
            if (fadeOutCoroutine != null) return;

            fadeOutCoroutine = StartCoroutine(FadeOutAndStop(m_bGAudioSource, fadeDuration));
        }
        private IEnumerator FadeOutAndStop(AudioSource audioSource, float duration)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / duration;
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume; // 重置音量以便于再次播放时使用

            fadeOutCoroutine = null; // 清除协程标记，表示淡出操作已完成
        }

        public void PlayBackgroundAudioRandomly()
        {
            if (bgAudioClips == null || bgAudioClips.Count == 0)
            {
                Debug.LogError("No background audio clips available to play.");
                goto End;
            }

            if (m_bGAudioSource == null)
            {
                Debug.LogError("Background AudioSource not assigned!");
                goto End;
            }

            // 从可用的音频剪辑中随机选择一个
            int randomIndex = UnityEngine.Random.Range(0, bgAudioClips.Count);
            AudioClip selectedClip = bgAudioClips[randomIndex];

            // 设置选定的clip并播放
            m_bGAudioSource.clip = selectedClip;
            m_bGAudioSource.Play();
        End:
            return;
        }
        public void PlayUIAudio(UIAudioClipType clipType)
        {
            if (m_uiAudioClipDictionary.TryGetValue(clipType, out var clip))
            {
                m_UIAudioSource.clip = clip;
                m_UIAudioSource.Play();
            }
            else
            {
                Debug.LogError($"Audio clip not found for type: {clipType}");
            }
        }

    }
}
