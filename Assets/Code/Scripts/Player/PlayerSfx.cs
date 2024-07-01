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
    }
}