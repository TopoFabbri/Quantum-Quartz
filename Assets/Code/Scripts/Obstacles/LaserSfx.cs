using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class LaserSfx : MonoBehaviour
    {
        [SerializeField] private string offEvent;
        [SerializeField] private string windUpEvent;
        [SerializeField] private bool playSound = true;
        
        public void Off()
        {
            if (!playSound) return;
            
            AkSoundEngine.PostEvent(offEvent, gameObject);
        }
        
        public void WindUp()
        {
            if (!playSound) return;
            
            AkSoundEngine.PostEvent(windUpEvent, gameObject);
        }
    }
}