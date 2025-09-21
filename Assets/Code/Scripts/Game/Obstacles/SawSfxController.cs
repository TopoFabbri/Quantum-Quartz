using AK.Wwise;
using Code.Scripts.Game.Behaviour;
using System.Collections.Generic;
using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Game.Obstacles
{
    public class SawSfxController : RoomComponent
    {
        private static readonly List<SawSfxController> ActiveSaws = new();
        private static GameObject _sfxSoundPoster;
        private const int MaxSaws = 10;

        [SerializeField] private RTPC sawSfxRtpc;
        [SerializeField] private ColorObjectController colorObjectController;
        [SerializeField] private Event sawSfxEvent;
        
        private void Start()
        {
            if (colorObjectController)
            {
                colorObjectController.Toggled += OnToggled;
            }
        }

        protected override void OnDestroy()
        {
            if (colorObjectController)
            {
                colorObjectController.Toggled -= OnToggled;
            }
        }

        private void OnToggled(bool on)
        {
        }

        private void AddSaw()
        {
            if (ActiveSaws.Count <= 0)
            {
                _sfxSoundPoster = gameObject;
                sawSfxEvent.Post(_sfxSoundPoster);
            }
            
            if (!ActiveSaws.Contains(this))
                ActiveSaws.Add(this);
            
            UpdateSawsSfx();
        }

        private void RemoveSaw()
        {
            ActiveSaws.Remove(this);
            UpdateSawsSfx();
            
            if (ActiveSaws.Count <= 0)
                sawSfxEvent.Stop(_sfxSoundPoster);
        }

        private void UpdateSawsSfx()
        {
            float value = ActiveSaws.Count / (float)MaxSaws * 100;
            sawSfxRtpc.SetValue(_sfxSoundPoster, value);
        }

        public override void OnActivate()
        {
            AddSaw();
        }

        public override void OnDeactivate()
        {
            RemoveSaw();
        }
    }
}