using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Level
{
    public class SoundZoneEvent : MonoBehaviour
    {
        [SerializeField] private WwiseEvent wwiseEvent;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.isTrigger && other.CompareTag("Player"))
                wwiseEvent.SetOn(SfxController.musicObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.isTrigger && other.CompareTag("Player"))
                wwiseEvent.SetOff(SfxController.musicObject);
        }
    }
}
