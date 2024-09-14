using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class LaserSfx : MonoBehaviour
    {
        [SerializeField] private string offEvent;
        [SerializeField] private string windUpEvent;
        
        private new UnityEngine.Camera camera;

        private bool isOn;
        
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

            if (!isOn && onScreen)
                WindUp();
        }

        /// <summary>
        /// Call turn off laser event
        /// </summary>
        public void Off()
        {
            isOn = false;
            AkSoundEngine.PostEvent(offEvent, gameObject);
        }

        /// <summary>
        /// Call start laser event
        /// </summary>
        public void WindUp()
        {
            if (!IsOnScreen())
                return;
            
            isOn = true;
            AkSoundEngine.PostEvent(windUpEvent, gameObject);
        }
        
        /// <summary>
        /// Check if laser is on screen
        /// </summary>
        /// <returns>True if on screen</returns>
        private bool IsOnScreen()
        {
            Vector3 viewPos = camera.WorldToViewportPoint(transform.position);
            bool onScreen = viewPos.z > 0 && viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1;
            
            return onScreen;
        }
    }
}