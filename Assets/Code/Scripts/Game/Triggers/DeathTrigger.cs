using Code.Scripts.Game.Interfaces;
using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Game.Triggers
{
    /// <summary>
    /// Manage object death
    /// </summary>
    public class DeathTrigger : MonoBehaviour
    {
        [SerializeField] private Event hitEvent;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IKillable killable)) return;

            hitEvent?.Post(gameObject);

            killable.Kill();
        }
    }
}