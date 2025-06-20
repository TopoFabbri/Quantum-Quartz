using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Platforms
{
    /// <summary>
    /// Manage each platform
    /// </summary>
    public class PlatformController : MonoBehaviour
    {
        private const float EdgeOffset = 1f;
        private const float RadiusOffset = 2f;
        private const float OffMultiplier = .5f;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem psOn;
        [SerializeField] private ParticleSystem psOff;
        [SerializeField] private float particleQty;
        [SerializeField] private bool solid;

        public Event matSoundEvent;

        private void Start()
        {
            ConfigureParticleSystem(psOn, 1);
            ConfigureParticleSystem(psOff, OffMultiplier);
        }

        private void ConfigureParticleSystem(ParticleSystem ps, float multiplier)
        {
            if (!ps) return;

            ParticleSystem.ShapeModule psShape = ps.shape;
            ParticleSystem.EmissionModule emission = ps.emission;

            if (solid)
            {
                Vector3 adjustedSize = spriteRenderer.bounds.size - new Vector3(EdgeOffset, EdgeOffset, 0f);
                psShape.scale = adjustedSize;
                emission.rateOverTime = CalculateAreaEmissionRate(adjustedSize) * multiplier;
            }
            else
            {
                float radius = CalculateRadius(spriteRenderer.bounds.size.x);
                psShape.radius = radius;
                emission.rateOverTime = CalculateRadialEmissionRate(radius) * multiplier;
            }
        }
        
        private void ClearPsOff() => psOff.Clear();
        
        private void ClearPsOn() => psOn.Clear();

        private static float CalculateRadius(float width) => (width - RadiusOffset) / 2f;

        private float CalculateAreaEmissionRate(Vector3 scale) => scale.x * scale.y * particleQty;

        private float CalculateRadialEmissionRate(float radius) => radius * particleQty;
    }
}