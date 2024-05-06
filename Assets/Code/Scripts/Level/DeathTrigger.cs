using System;
using UnityEngine;

namespace Code.Scripts.Level
{
    /// <summary>
    /// Manage player death
    /// </summary>
    public class DeathTrigger : MonoBehaviour
    {
        public static event Action Death;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                Death?.Invoke();
        }
    }
}