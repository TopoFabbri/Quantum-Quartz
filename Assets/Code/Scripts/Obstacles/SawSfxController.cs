using System.Collections.Generic;
using AK.Wwise;
using Code.Scripts.Level;
using Code.Scripts.Platforms;
using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Obstacles
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
            colorObjectController.Toggled += OnToggled;
        }

        protected override void OnDestroy()
        {
            colorObjectController.Toggled -= OnToggled;
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