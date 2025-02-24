
using System;

namespace HSFramework.Setting
{
    public class BaseSettingData
    {
        public virtual void Reset()
        {

        }
    }


    [Serializable]
    public class VideoSettingData : BaseSettingData
    {
        public bool isFull;
        public int resolutionIdx;
        public int resolutionWidth;
        public int resolutionHeight;

        public VideoSettingData()
        {
            Reset();
        }

        public override void Reset() {
            isFull = true;
            resolutionIdx = 0;
            resolutionWidth = 1920;
            resolutionHeight = 1080;
        }
    }

    [Serializable]
    public class AudioSettingData : BaseSettingData
    {
        public float masterVolume;
        public float bgVolume;
        public float uiVolume;

        public AudioSettingData()
        {
            Reset();
        }

        public override void Reset()
        {
            masterVolume = 0.5f;
            bgVolume = 0.5f;
            uiVolume = 0.5f;
        }
    }

    [Serializable]
    public class GameSettingDatas
    {
        public VideoSettingData videoSetting;
        public AudioSettingData audioSetting;

        public GameSettingDatas()
        {
            videoSetting = new VideoSettingData();
            audioSetting = new AudioSettingData();
        }

    }
}
