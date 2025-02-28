using System.Collections.Generic;
using UnityEngine;

public class UnitAudioManager : MonoBehaviour
{
    public List<AudioClip> defalutFootAudioClips = new();
    private AudioSource m_audioSource;
    private Transform m_thisTransform;

    [Tooltip("How far the ray is casted.")]
    [SerializeField] float groundCheckDistance = 1f;
    [Tooltip("What are the layers that should be taken into account when checking for ground.")]
    [SerializeField] LayerMask groundLayers;


    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        m_thisTransform = transform;
    }
    public void Init()
    {
    }


    private RaycastHit _groundHit;
    public void PlayFootAudioClip()
    {
        Ray checkerRay = new Ray(m_thisTransform.position + (Vector3.up * 0.1f), Vector3.down);

        if (Physics.Raycast(checkerRay, out _groundHit, groundCheckDistance))
        {
            if (_groundHit.collider == null)
            {
                // 可能在悬崖边
                goto End;
            }
            var clip = FootStepsDatabase.singleton.GetFootstep(_groundHit);
            if (clip != null)
            {
                m_audioSource.PlayOneShot(clip);
            }
            else
            {
                var index = Random.Range(0, defalutFootAudioClips.Count);
                m_audioSource.PlayOneShot(defalutFootAudioClips[index]);
            }
        }
    End:
        return;
    }

}
