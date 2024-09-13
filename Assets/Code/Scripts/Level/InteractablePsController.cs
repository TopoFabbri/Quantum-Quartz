using UnityEngine;

namespace Code.Scripts.Level
{
    public class InteractablePsController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ps;

        [Header("Ps Settings")]
        [SerializeField] private bool allowMultiplePlays = true;
        
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                PlayPs();
        }

        private void PlayPs()
        {
            bool canPlay = (allowMultiplePlays ? !ps.isEmitting : !ps.IsAlive());
            
            if (!canPlay)
                return;
        
            ps.Play();
        }
    }
}
