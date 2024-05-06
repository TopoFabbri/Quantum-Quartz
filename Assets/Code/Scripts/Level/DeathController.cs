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

        private void OnEnable()
        {
            DeathTrigger.Death += OnDeathHandler;
        }
        
        private void OnDisable()
        {
            DeathTrigger.Death -= OnDeathHandler;
        }
        
        public void CheckPoint(Vector2 position)
        {
            checkPoint = position;
        }
        
        /// <summary>
        /// Respawn player position on death
        /// </summary>
        private void OnDeathHandler()
        {
            transform.position = checkPoint;
        }
    }
}