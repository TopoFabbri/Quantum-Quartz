using AYellowpaper.SerializedCollections;
using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Platforms
{
    /// <summary>
    /// Manage each platform
    /// </summary>
    [ExecuteInEditMode]
    public class PlatformController : MonoBehaviour
    {
        public enum AnimationFrame
        {
            Off,
            On,
            TurnOff1,
            TurnOff2,
            TurnOff3,
            TurnOff4,
            TurnOn1,
            TurnOn2,
            TurnOn3,
            TurnOn4
        }

        private const float EdgeOffset = 1f;
        private const float RadiusOffset = 2f;
        private const float OffMultiplier = .5f;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem psOn;
        [SerializeField] private ParticleSystem psOff;
        [SerializeField] private float particleQty;
        [SerializeField] private bool solid;
        [SerializeField] private AnimationFrame curFrame = AnimationFrame.Off;
        [SerializeField] private SerializedDictionary<AnimationFrame, Sprite> frameMapping;

        public Event matSoundEvent;
        private AnimationFrame? prevFrame = null;

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (psOn)
                {
                    ConfigureParticleSystem(psOn, 1);
                }
                if (psOff)
                {
                    ConfigureParticleSystem(psOff, OffMultiplier);
                }
            }
        }

        private void Update()
        {
            if (!prevFrame.HasValue || prevFrame.Value != curFrame)
            {
                Vector2 size = spriteRenderer.size;
                spriteRenderer.sprite = frameMapping.TryGetValue(curFrame, out Sprite sprite) ? sprite : null;
                spriteRenderer.size = size;
            }
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

        private static float CalculateRadius(float width) => (width - RadiusOffset) / 2f;

        private float CalculateAreaEmissionRate(Vector3 scale) => scale.x * scale.y * particleQty;

        private float CalculateRadialEmissionRate(float radius) => radius * particleQty;
    }
}