using System.Collections.Generic;
using UnityEngine;

public class UnitAudioManager : MonoBehaviour
{
    public List<AudioClip> defalutFootAudioClips = new();
    private AudioSource m_audioSource;
    private Transform m_thisTransform;
    private RaycastHit m_currentGroundInfo;


    [Tooltip("If this is enabled, you can see how far the script will check for ground, and the radius of the check.")]
    [SerializeField] bool debugMode = true;
    [Tooltip("How high, relative to the character's pivot point the start of the ray is.")]
    [SerializeField] float groundCheckHeight = 0.5f;
    [Tooltip("What is the radius of the ray.")]
    [SerializeField] float groundCheckRadius = 0.5f;
    [Tooltip("How far the ray is casted.")]
    [SerializeField] float groundCheckDistance = 0.3f;
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
    void OnDrawGizmos()
    {
        if (debugMode)
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up * groundCheckHeight, groundCheckRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + Vector3.up * groundCheckHeight, Vector3.down * (groundCheckDistance + groundCheckRadius));
        }
    }


    public void PlayFootAudioClip()
    {
        Ray ray = new Ray(m_thisTransform.position + Vector3.up * groundCheckHeight, Vector3.down);
        Physics.SphereCast(ray, groundCheckRadius, out m_currentGroundInfo, groundCheckDistance, groundLayers, QueryTriggerInteraction.Ignore);

        if(m_currentGroundInfo.collider == null)
        {
            // 可能在悬崖边
            return;
        }
        var clip = SurfaceManager.singleton.GetFootstep(m_currentGroundInfo.collider, m_currentGroundInfo.point);
        if(clip != null)
        {
            m_audioSource.PlayOneShot(clip);
        }
        else
        {
            var index = Random.Range(0, defalutFootAudioClips.Count);
            m_audioSource.PlayOneShot(defalutFootAudioClips[index]);
        }
    }
}
