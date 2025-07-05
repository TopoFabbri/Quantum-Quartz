using Code.Scripts.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Level
{
    public abstract class InteractableComponent : MonoBehaviour
    {
        public virtual bool RequiresClick => false;
        
        private bool awaitingInteraction = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.isTrigger && other.gameObject.CompareTag("Player"))
            {
                if (!RequiresClick)
                {
                    OnInteracted();
                }
                else if (other.TryGetComponent(out PlayerController player))
                {
                    player.EnterInteractable(this);
                    awaitingInteraction = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (RequiresClick && !other.isTrigger && other.gameObject.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
            {
                player.ExitInteractable(this);
                awaitingInteraction = false;
            }
        }

        public bool Interact()
        {
            if (awaitingInteraction && RequiresClick)
            {
                OnInteracted();
                return true;
            }
            return false;
        }

        protected abstract void OnInteracted();
    }
}