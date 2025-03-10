using UnityEngine;
using UnityEngine.Audio;

/*Script, that changes the snapshot 
depending on the player's position 
outside or inside trigger zone*/

namespace Suntail
{
    public class AudioZoneControl : MonoBehaviour
    {
        [Tooltip("Snapshot for outdoor enviroment")]
        [SerializeField] private AudioMixerSnapshot outdoorSnapshot;

        [Tooltip("Snapshot for indoor enviroment")]
        [SerializeField] private AudioMixerSnapshot indoorSnapshot;

        [Tooltip("Transition time between snapshots")]
        [SerializeField] private float crossfadeTime = 0.5f;

        [Tooltip("Trigger tag for updating audio zones")]
        [SerializeField] private string triggerTag = "Player";

        //Private variables
        private int zoneCount;

        //Trigger, changing snapshot when player enter zone
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == triggerTag)
            {
                zoneCount += 1;
                UpdateAudioZoneSnapshot();
            }
        }

        //Trigger, changing snapshot when player leave zone
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == triggerTag)
            {
                zoneCount -= 1;
                UpdateAudioZoneSnapshot();
            }
        }

        //Update snapshot, depending on the location
        private void UpdateAudioZoneSnapshot()
        {
            if (zoneCount > 0)
            {
                indoorSnapshot.TransitionTo(crossfadeTime);
            }
            else
            {
                outdoorSnapshot.TransitionTo(crossfadeTime);
            }
        }
    }
}