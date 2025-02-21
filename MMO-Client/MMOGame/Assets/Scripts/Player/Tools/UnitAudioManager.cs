using System.Collections.Generic;
using UnityEngine;

public class UnitAudioManager : MonoBehaviour
{
    private AudioSource m_audioSource;
    public List<AudioClip> footAudioClips = new();

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootAudioClip()
    {
        if (footAudioClips.Count > 0)
        {
            var index = Random.Range(0, footAudioClips.Count);
            m_audioSource.PlayOneShot(footAudioClips[index]);
        }
    }

    public void Init()
    {
    }
}
