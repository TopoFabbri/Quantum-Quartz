using UnityEngine;

namespace Code.Scripts.Game.Visuals
{
    public class InteractablePsController : InteractableComponent
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
