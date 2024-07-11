using UnityEngine;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage
    /// </summary>
    public class PlayerSfx : MonoBehaviour
    {
        [SerializeField] private string stepEvent;
        [SerializeField] private string jumpEvent;
        [SerializeField] private string landEvent;
        [SerializeField] private string djmpEvent;
        
        /// <summary>
        /// Call step event
        /// </summary>
        public void Step()
        {
            AkSoundEngine.PostEvent(stepEvent, gameObject);
        }
        
        /// <summary>
        /// Call jump event
        /// </summary>
        public void Jump()
        {
            AkSoundEngine.PostEvent(jumpEvent, gameObject);
        }
        
        public void Land()
        {
            AkSoundEngine.PostEvent(landEvent, gameObject);
        }
        
        public void Djmp()
        {
            AkSoundEngine.PostEvent(djmpEvent, gameObject);
        }
    }
}