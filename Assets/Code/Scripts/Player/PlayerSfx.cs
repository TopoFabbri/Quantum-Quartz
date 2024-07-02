using UnityEngine;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage
    /// </summary>
    public class PlayerSfx : MonoBehaviour
    {
        /// <summary>
        /// Call step event
        /// </summary>
        public void Step()
        {
            AkSoundEngine.PostEvent("Play_Lab_Metal_Footsteps", gameObject);
        }
        
        /// <summary>
        /// Call jump event
        /// </summary>
        public void Jump()
        {
            AkSoundEngine.PostEvent("Play_Basic_Jump", gameObject);
        }
        
        public void Land()
        {
            AkSoundEngine.PostEvent("Play_Lab_Landing", gameObject);
        }
        
        public void Djmp()
        {
            AkSoundEngine.PostEvent("Play_Blue_Quartz_Jump", gameObject);
        }
    }
}