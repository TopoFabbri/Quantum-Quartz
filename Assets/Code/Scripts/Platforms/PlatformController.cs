using System;
using UnityEngine;

namespace Code.Scripts.Platforms
{
    /// <summary>
    /// Manage each platform
    /// </summary>
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private float particleQty;
        [SerializeField] private bool solid;

        public string matSoundEvent;
        
        private void Start()
        {
            if (!ps) return;
            
            ParticleSystem.ShapeModule psShape = ps.shape;
            ParticleSystem.EmissionModule module = ps.emission;

            if (solid)
            {
                psShape.scale = spriteRenderer.bounds.size - new Vector3(1f, 1f, 0f);
                module.rateOverTime = psShape.scale.x * psShape.scale.y * particleQty;
            }
            else
            {
                psShape.radius = (spriteRenderer.bounds.size.x - 2f) / 2f;
                module.rateOverTime = psShape.radius * particleQty;
            }
        }
    }
}