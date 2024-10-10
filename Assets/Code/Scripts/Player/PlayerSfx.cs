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
        
        /// <summary>
        /// Call land event
        /// </summary>
        public void Land()
        {
            AkSoundEngine.PostEvent(landEvent, gameObject);
        }
        
        /// <summary>
        /// Call djmp event
        /// </summary>
        public void Djmp()
        {
            AkSoundEngine.PostEvent(djmpEvent, gameObject);
        }

        /// <summary>
        /// Call left dash event
        /// </summary>
        public void DashL()
        {
            AkSoundEngine.PostEvent("Play_Dash_L", gameObject);
        }
        
        /// <summary>
        /// Call right dash event
        /// </summary>
        public void DashR()
        {
            AkSoundEngine.PostEvent("Play_Dash_R", gameObject);
        }

        /// <summary>
        /// Call glide event
        /// </summary>
        public void PlayGlide()
        {
            AkSoundEngine.PostEvent("Play_Yellow_Quartz", gameObject);
        }

        /// <summary>
        /// Call stop glide event
        /// </summary>
        public void StopGlide()
        {
            AkSoundEngine.PostEvent("Stop_Yellow_Quartz", gameObject);
        }
        
        /// <summary>
        /// Call death event
        /// </summary>
        public void Death()
        {
            AkSoundEngine.PostEvent("Play_Death", gameObject);
        }
    }
}