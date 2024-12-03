using BaseSystem.Singleton;
using UnityEngine;

namespace GameClient.HSFramework
{
    public class GlobalAudioManager : Singleton<GlobalAudioManager>
    {
        private AudioSource m_backgroundMusicSource;

        protected override void Awake()
        {
            base.Awake();

            m_backgroundMusicSource = GetComponent<AudioSource>();
            if (m_backgroundMusicSource == null)
            {
                m_backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayBackgroundMusic(string clipName)
        {
            //todo 用对象池
            AudioClip clip = Res.LoadAssetSync<AudioClip>(clipName);

            if (clip != null)
            {
                m_backgroundMusicSource.clip = clip;
                m_backgroundMusicSource.loop = true;
                m_backgroundMusicSource.Play();
            }
            else
            {
                Debug.LogError("Audio clip not found: " + clipName);
            }
        }

        public void StopBackgroundMusic()
        {
            m_backgroundMusicSource.Stop();
        }

    }
}
