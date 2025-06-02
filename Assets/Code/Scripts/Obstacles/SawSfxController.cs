using System.Collections.Generic;
using AK.Wwise;
using Code.Scripts.Platforms;
using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Obstacles
{
    public class SawSfxController : MonoBehaviour
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

        private void Update()
        {
            if (!ActiveSaws.Contains(this))
                return;
            
            if (!IsOnScreen())
                RemoveSaw();
        }

        private void OnToggled(bool on)
        {
            if (!on)
            {
                RemoveSaw();
                return;
            }
            
            if (IsOnScreen())
                AddSaw();
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
        
        private bool IsOnScreen()
        {
            if (!UnityEngine.Camera.main)
                return false;

            Vector3 viewPos = UnityEngine.Camera.main.WorldToViewportPoint(transform.position);
            bool onScreen = viewPos is { z: > 0, x: > 0 and < 1, y: > 0 and < 1 };

            return onScreen;
        }
    }
}