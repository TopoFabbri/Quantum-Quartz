using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class LaserSfx : MonoBehaviour
    {
        [SerializeField] private string offEvent;
        [SerializeField] private string windUpEvent;
        public void Off()
        {
            AkSoundEngine.PostEvent(offEvent, gameObject);
        }
        
        public void WindUp()
        {
            AkSoundEngine.PostEvent(windUpEvent, gameObject);
        }
    }
}