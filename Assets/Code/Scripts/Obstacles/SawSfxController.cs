using System.Collections.Generic;
using AK.Wwise;
using Code.Scripts.Platforms;
using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class SawSfxController : MonoBehaviour
    {
        private static readonly List<SawSfxController> ActiveSaws = new();
        private const int MaxSaws = 10;

        [SerializeField] private RTPC sawSfxRtpc;
        [SerializeField] private ColorObjectController colorObjectController;

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
            ActiveSaws.Add(this);
            UpdateSawsSfx();
        }

        private void RemoveSaw()
        {
            ActiveSaws.Remove(this);
            UpdateSawsSfx();
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