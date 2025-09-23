using Code.Scripts.Game.Managers;
using System.Collections.Generic;
using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Game.Obstacles
{
    public class LaserSfx : RoomComponent
    {
        [SerializeField] private Event laserLoopEvent;
        [SerializeField] private Event windUpEvent;
        [SerializeField] private Event startShootEvent;
        
        private bool isOn;
        private bool isActive;

        private static readonly List<LaserSfx> LasersPlaying = new();

        /// <summary>
        /// Call turn off laser event
        /// </summary>
        public void Off()
        {
            if (!isOn) return;
            
            isOn = false;
            
            if (isActive)
                RemoveFromPlaying();
        }

        /// <summary>
        /// Call turn on laser event
        /// </summary>
        private void On()
        {
            if (isOn) return;
            isOn = true;
            
            if (!isActive) return;

            startShootEvent.Post(gameObject);
            AddToPlaying();
        }

        /// <summary>
        /// Call start laser event
        /// </summary>
        public void WindUp()
        {
            if (isActive)
                windUpEvent.Post(gameObject);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (isOn)
                LasersPlaying.Remove(this);
            
            SfxController.StopAllOn(gameObject);
        }
        
        private void AddToPlaying()
        {
            if (LasersPlaying.Count <= 0)
                laserLoopEvent.Post(gameObject);
            
            if (LasersPlaying.Contains(this)) return;
            
            LasersPlaying.Add(this);
        }

        private void RemoveFromPlaying()
        {
            if (LasersPlaying.Count <= 0) return;
            
            LasersPlaying.Remove(this);
            
            if (LasersPlaying.Count <= 0)
                laserLoopEvent.Stop(gameObject);
        }

        public override void OnActivate()
        {
            isActive = true;
            
            if (isOn)
                AddToPlaying();
        }

        public override void OnDeactivate()
        {
            isActive = false;
            
            if (isOn)
                RemoveFromPlaying();
        }
    }
}