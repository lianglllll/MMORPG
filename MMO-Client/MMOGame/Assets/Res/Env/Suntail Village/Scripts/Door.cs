using System.Collections;
using UnityEngine;

/*Sub-component of the main player interaction script, 
  door animation is played by the animator*/

namespace Suntail
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        [Tooltip("Door opening sound")]
        [SerializeField] private AudioClip openSound;

        [Tooltip("Door closing sound")]
        [SerializeField] private AudioClip closeSound;

        [Tooltip("Additional delay in deactivating interaction, added to animation time")]
        [SerializeField] private float doorDelayTime;
        [HideInInspector] public bool doorOpen = false;

        //Private variables.
        private Animator _doorAnimator;
        private AudioSource _doorAudioSource;
        private float _doorOpenTime;
        private bool _pauseInteraction;

        private void Awake()
        {
            _doorAudioSource = gameObject.GetComponent<AudioSource>();
            _doorAnimator = gameObject.GetComponent<Animator>();
            _doorOpenTime = _doorAnimator.GetCurrentAnimatorStateInfo(0).length + doorDelayTime; //Sum of animation time and additional delay
        }

        //Play an animation and sound, depending on door status
        public void PlayDoorAnimation()
        {
            if (!doorOpen && !_pauseInteraction)
            {
                _doorAnimator.Play("OpenDoor");
                _doorAudioSource.clip = openSound;
                doorOpen = true;
                _doorAudioSource.Play();
                StartCoroutine(PauseInteraction());

            }
            else if (doorOpen && !_pauseInteraction)
            {
                _doorAudioSource.clip = closeSound;
                _doorAnimator.Play("CloseDoor");
                doorOpen = false;
                _doorAudioSource.Play();
                StartCoroutine(PauseInteraction());

            }

        }

        //Waiting for door open time, to prevent the door from opening/closing again
        private IEnumerator PauseInteraction()
        {
            _pauseInteraction = true;
            yield return new WaitForSeconds(_doorOpenTime);
            _pauseInteraction = false;
        }

    }
}