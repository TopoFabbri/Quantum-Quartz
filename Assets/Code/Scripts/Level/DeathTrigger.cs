using Code.Scripts.Interfaces;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level
{
    /// <summary>
    /// Manage object death
    /// </summary>
    public class DeathTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IKillable killable))
                killable.Kill();
        }
    }
}