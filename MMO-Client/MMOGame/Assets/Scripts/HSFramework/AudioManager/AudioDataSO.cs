using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class SimpleAudioDataConf
{
    public string key;
    public string path;
    public bool loop;
    public string mixerName;
}

[Serializable]
public class AudioDataConfig
{
    public List<SimpleAudioDataConf> ConfList = new List<SimpleAudioDataConf>();
}

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "AudioDataSO", order = 0)]
public class AudioDataSO : ScriptableObject
{
    public string Desc = "该配置是自动生成的，请勿手动修改！"; 
    [SerializeField]
    public AudioDataConfig Conf;
}
