using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Level
{
    public class SoundZoneEvent : MonoBehaviour
    {
        [SerializeField] private WwiseEvent wwiseEvent;
        [SerializeField] private string playerTag = "Player";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
                wwiseEvent.SetOn(SfxController.musicObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
                wwiseEvent.SetOff(SfxController.musicObject);
        }
    }
}
