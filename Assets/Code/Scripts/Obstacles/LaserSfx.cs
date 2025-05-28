using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Obstacles
{
    public class LaserSfx : MonoBehaviour
    {
        [SerializeField] private Event laserEvent;
        [SerializeField] private Event windUpEvent;
        [SerializeField] private Event shootEvent;

        private new UnityEngine.Camera camera;

        private bool isOn;
        private uint laserEventId;

        private void Start()
        {
            if (UnityEngine.Camera.main)
                camera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            bool onScreen = IsOnScreen();

            if (isOn && !onScreen)
                Off();
        }

        /// <summary>
        /// Call turn off laser event
        /// </summary>
        public void Off()
        {
            if (!isOn)
                return;
            
            isOn = false;
            
            laserEvent.Stop(gameObject);
            shootEvent.Stop(gameObject);
        }

        /// <summary>
        /// Call start laser event
        /// </summary>
        public void WindUp()
        {
            if (!IsOnScreen())
                return;

            windUpEvent.Post(gameObject);
        }

        /// <summary>
        /// Call turn on laser event
        /// </summary>
        private void On()
        {
            if (isOn || !IsOnScreen())
                return;
            
            isOn = true;

            shootEvent.Post(gameObject);
            laserEvent.Post(gameObject);
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