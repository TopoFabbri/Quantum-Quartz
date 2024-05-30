using UnityEngine;

namespace Code.Scripts.Level
{
    /// <summary>
    /// Manage on death player actions
    /// </summary>
    public class DeathController : MonoBehaviour
    {
        private Vector2 checkPoint;

        private void Start()
        {
            checkPoint = transform.position;
        }
        
        public void CheckPoint(Vector2 position)
        {
            checkPoint = position;
        }
        
        /// <summary>
        /// Respawn player position on death
        /// </summary>
        public void Die()
        {
            transform.position = checkPoint;
        }
    }
}