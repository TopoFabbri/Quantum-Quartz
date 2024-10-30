using Code.Scripts.Colors;
using UnityEngine;

namespace Code.Scripts.Platforms
{
    /// <summary>
    /// Manage each platform
    /// </summary>
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ColorSwitcher.QColor qColor;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private float particleQty;
        [SerializeField] private bool solid;

        public string matSoundEvent;
        
        private static readonly int On = Animator.StringToHash("On");

        private void Start()
        {
            ToggleColor(ColorSwitcher.QColor.None);

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

        private void OnEnable()
        {
            ColorSwitcher.ColorChanged += ToggleColor;
        }

        private void OnDisable()
        {
            ColorSwitcher.ColorChanged -= ToggleColor;
        }

        /// <summary>
        /// Toggle platform on and off depending on color
        /// </summary>
        /// <param name="color">New world color</param>
        private void ToggleColor(ColorSwitcher.QColor color)
        {
            if (color == qColor || qColor == ColorSwitcher.QColor.None)
                Activate();
            else
                Deactivate();
        }

        /// <summary>
        /// Toggle platform on
        /// </summary>
        private void Activate()
        {
            animator.SetBool(On, true);
        }

        /// <summary>
        /// toggle platform off
        /// </summary>
        private void Deactivate()
        {
            animator.SetBool(On, false);
        }
    }
}