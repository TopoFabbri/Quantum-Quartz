using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Obstacles
{
    public class LaserSfx : MonoBehaviour
    {
        [SerializeField] private Event laserLoopEvent;
        [SerializeField] private Event windUpEvent;
        [SerializeField] private Event startShootEvent;

        private new UnityEngine.Camera camera;

        private bool isOn;
        private bool isOnScreen;
        private bool playingSound;
        private uint laserEventId;

        private static int _lasersPlayingCount;

        private void Start()
        {
            if (UnityEngine.Camera.main)
                camera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            isOnScreen = IsOnScreen();

            if (isOnScreen && !playingSound)
            {
                if (isOn)
                    On();
                else
                    Off();
            }
            else if (!isOnScreen)
            {
                startShootEvent.Stop(gameObject);
                windUpEvent.Stop(gameObject);
                RemoveLaserPlaying();
            }
        }

        /// <summary>
        /// Call turn off laser event
        /// </summary>
        public void Off()
        {
            isOn = false;

            startShootEvent.Stop(gameObject);

            RemoveLaserPlaying();

            playingSound = true;
        }

        /// <summary>
        /// Call start laser event
        /// </summary>
        public void WindUp()
        {
            if (!isOnScreen)
                return;

            playingSound = true;

            windUpEvent.Post(gameObject);
        }

        /// <summary>
        /// Call turn on laser event
        /// </summary>
        private void On()
        {
            if (!isOn && isOnScreen)
                startShootEvent.Post(gameObject);

            isOn = true;
            
            if (!isOnScreen)
                return;

            AddLaserPlaying();

            playingSound = true;
        }

        private void AddLaserPlaying()
        {
            if (_lasersPlayingCount <= 0)
            {
                _lasersPlayingCount = 1;
                laserLoopEvent.Post(gameObject);
                return;
            }
            
            _lasersPlayingCount++;
        }

        private void RemoveLaserPlaying()
        {
            _lasersPlayingCount--;

            if (_lasersPlayingCount > 0) return;
            
            _lasersPlayingCount = 0;
            laserLoopEvent.Stop(gameObject);
        }
        
        /// <summary>
        /// Check if laser is on screen
        /// </summary>
        /// <returns>True if on screen</returns>
        private bool IsOnScreen()
        {
            if (!camera)
                return false;

            Vector3 viewPos = camera.WorldToViewportPoint(transform.position);
            bool onScreen = viewPos is { z: > 0, x: > 0 and < 1, y: > 0 and < 1 };

            return onScreen;
        }

        private void OnDestroy()
        {
            Off();
        }
    }
}