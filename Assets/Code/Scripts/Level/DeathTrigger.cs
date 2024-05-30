using UnityEngine;

namespace Code.Scripts.Level
{
    /// <summary>
    /// Manage player death
    /// </summary>
    public class DeathTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (TryGetComponent(out DeathController deathController))
                deathController.Die();
        }
    }
}