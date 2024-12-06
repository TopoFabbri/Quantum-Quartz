using UnityEngine;

namespace Code.Scripts.Level
{
    public class InteractablePsController : InteractableController2D
    {
        [SerializeField] private ParticleSystem ps;

        [Header("Ps Settings")]
        [SerializeField] private bool allowMultiplePlays = true;

        protected override void OnInteracted()
        {
            bool canPlay = allowMultiplePlays ? !ps.isEmitting : !ps.IsAlive();
            
            if (!canPlay)
                return;
        
            ps.Play();
        }
    }
}
