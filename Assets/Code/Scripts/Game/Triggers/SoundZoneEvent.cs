using Code.Scripts.Game.Managers;
using UnityEngine;

namespace Code.Scripts.Game.Triggers
{
    public class SoundZoneEvent : InteractableComponent
    {
        [SerializeField] private WwiseEvent wwiseEvent;

        protected override void OnInteracted()
        {
            wwiseEvent.SetOn(SfxController.musicObject);
        }

        protected override void OnStopInteraction()
        {
            wwiseEvent.SetOff(SfxController.musicObject);
        }
    }
}
