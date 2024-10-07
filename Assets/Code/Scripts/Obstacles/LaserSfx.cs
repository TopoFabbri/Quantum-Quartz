using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class LaserSfx : MonoBehaviour
    {
        [SerializeField] private string laserEvent;
        [SerializeField] private string windUpEvent;
        [SerializeField] private string shootEvent;

        private new UnityEngine.Camera camera;

        private bool isOn;
        private uint laserEventId;

        private void Awake()
        {
            if (UnityEngine.Camera.main != null)
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
            
            AkSoundEngine.StopPlayingID(laserEventId);
        }

        /// <summary>
        /// Call start laser event
        /// </summary>
        public void WindUp()
        {
            if (!IsOnScreen())
                return;

            AkSoundEngine.PostEvent(windUpEvent, gameObject);
        }

        /// <summary>
        /// Call turn on laser event
        /// </summary>
        private void On()
        {
            if (isOn || !IsOnScreen())
                return;
            
            isOn = true;

            AkSoundEngine.PostEvent(shootEvent, gameObject);
            laserEventId = AkSoundEngine.PostEvent(laserEvent, gameObject);
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
            bool onScreen = viewPos.z > 0 && viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1;

            return onScreen;
        }
    }
}