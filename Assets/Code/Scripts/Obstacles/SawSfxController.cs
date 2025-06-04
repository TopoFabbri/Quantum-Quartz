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
        private const int MaxSaws = 10;

        [SerializeField] private RTPC sawSfxRtpc;
        [SerializeField] private ColorObjectController colorObjectController;
        [SerializeField] private Event sawSfxEvent;
        
        private void Start()
        {
            colorObjectController.Toggled += OnToggled;
        }

        private void OnDestroy()
        {
            colorObjectController.Toggled -= OnToggled;
        }

        private void OnToggled(bool on)
        {
        }

        private void AddSaw()
        {
            if (ActiveSaws.Count <= 0)
                sawSfxEvent.Post(gameObject);
            
            if (!ActiveSaws.Contains(this))
                ActiveSaws.Add(this);
            
            UpdateSawsSfx();
        }

        private void RemoveSaw()
        {
            ActiveSaws.Remove(this);
            UpdateSawsSfx();
            
            if (ActiveSaws.Count <= 0)
                sawSfxEvent.Stop(gameObject);
        }

        private void UpdateSawsSfx()
        {
            float value = ActiveSaws.Count / (float)MaxSaws * 100;
            sawSfxRtpc.SetValue(gameObject, value);
        }

        public override void OnActivate()
        {
            AddSaw();
        }

        public override void OnDeactivate()
        {
            RemoveSaw();
        }

        public override void OnUpdate()
        {
        }
    }
}