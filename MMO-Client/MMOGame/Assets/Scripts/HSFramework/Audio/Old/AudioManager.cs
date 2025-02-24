using HSFramework.MyDelayedTaskScheduler;
using HSFramework.PoolModule;
using HSFramework.Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-99)]
public class AudioManager : Singleton<AudioManager>
{
    public AudioDataSO audioDataSo;
    public AudioMixer audioMixer;
    private Transform _transform;
    private Dictionary<string, SimpleAudioDataConf> audioDic = new Dictionary<string, SimpleAudioDataConf>();
    private List<AudioSource> _playingAudioSources = new List<AudioSource>();

    private static string BgVolumeKey = "BgVolumeKey";
    private static string UIVolumeKey = "UIVolumeKey";

    protected override void Awake()
    {
        base.Awake();
        _transform = transform;
        audioDic.Clear();
        if(audioDataSo)
        {
            foreach (var audioData in audioDataSo.Conf.ConfList)
            {
                audioDic.Add(audioData.key, audioData);
            }
        }
        UnityObjectPoolFactory.Instance.LoadFuncDelegate = PoolAssetLoad.LoadAssetByYoo<UnityEngine.Object>;


    }
    private void Start()
    {
        //音频初始化设置
        float volume = 0;
        if (PlayerPrefs.HasKey(BgVolumeKey))
        {
            volume = PlayerPrefs.GetFloat(BgVolumeKey);
        }
        audioMixer.SetFloat("Bg", volume);

        if (PlayerPrefs.HasKey(UIVolumeKey))
        {
            volume = PlayerPrefs.GetFloat(UIVolumeKey);
        }
        audioMixer.SetFloat("UI", volume);

    }

    //播放音频
    public AudioSource PlayAudio(string key, Transform audioParent = null)
    {
        if (audioDic.TryGetValue(key, out SimpleAudioDataConf conf))
        {
            AudioClip clip = Res.LoadAssetSync<AudioClip>(conf.path);

            if (clip)
            {
                GameObject objAudioSource = UnityObjectPoolFactory.Instance.GetItem<GameObject>("Audios/AudioSourceTemplate.prefab");

                AudioSource audioSource = objAudioSource != null ? objAudioSource.GetComponent<AudioSource>() : null;
                if (audioSource)
                {
                    if(audioParent)
                        audioSource.transform.SetParent(audioParent, false);
                    else
                        audioSource.transform.SetParent(_transform, false);
                    audioSource.transform.localPosition = Vector3.zero;
                    audioSource.gameObject.SetActive(true);
                    audioSource.clip = clip;
                    audioSource.loop = conf.loop;
                    var groups = audioMixer.FindMatchingGroups(conf.mixerName);
                    audioSource.outputAudioMixerGroup = groups.Length > 0 ? groups[0] : null;
                    audioSource.Play();
                    _playingAudioSources.Add(audioSource);
                    if (!conf.loop)
                    {
                        DelayedTaskScheduler.Instance.AddDelayedTask(
                            TimerUtil.GetLaterMilliSecondsBySecond(clip.length),
                            () =>
                            {
                                RecycleAudioHandle(audioSource);
                            });
                    }
                }

                return audioSource;
            }
        }

        return null;
    }

    //音频回收
    public void RecycleAudio(AudioSource audioSource)
    {
        RecycleAudioHandle(audioSource);
    }
    private void RecycleAudioHandle(AudioSource audioSource)
    {
        if(audioSource && _playingAudioSources.Contains(audioSource))
        {
            audioSource.Stop();
            _playingAudioSources.Remove(audioSource);
            if(audioSource.gameObject != null)
            {
                audioSource.gameObject.SetActive(false);
                UnityObjectPoolFactory.Instance.RecycleItem("Audios/AudioSourceTemplate.prefab", audioSource.gameObject);
                audioSource.transform.SetParent(_transform, false);
            }
        }
    }

    //获取音量大小
    public float GetMusicVolume()
    {
        float volume = 1;
        audioMixer.GetFloat("MusicVolume", out volume);
        return volume;
    }
    public float GetSFXVolume()
    {
        float volume = 1;
        audioMixer.GetFloat("SFXVolume", out volume);
        return volume;
    }

    //设置音量大小
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat(BgVolumeKey, volume);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat(UIVolumeKey, volume);
    }
}
