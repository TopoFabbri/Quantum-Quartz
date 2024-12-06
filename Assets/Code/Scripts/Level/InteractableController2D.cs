using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Level
{
    public abstract class InteractableController2D : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                OnInteracted();
        }

        protected abstract void OnInteracted();
    }
}