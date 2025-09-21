using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Game
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
                    OnAwaitingInteraction(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.isTrigger && other.gameObject.CompareTag("Player"))
            {
                if (!RequiresClick)
                {
                    OnStopInteraction();
                }
                else if (other.TryGetComponent(out PlayerController player))
                {
                    player.ExitInteractable(this);
                    OnAwaitingInteraction(false);
                }
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

        protected virtual void OnStopInteraction() { }

        protected virtual void OnAwaitingInteraction(bool awaitingInteraction)
        {
            this.awaitingInteraction = awaitingInteraction;
        }
    }
}