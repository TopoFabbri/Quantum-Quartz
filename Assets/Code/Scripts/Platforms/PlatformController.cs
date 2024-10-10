using System;
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
        
        private static readonly int On = Animator.StringToHash("On");

        private void Start()
        {
            ToggleColor(ColorSwitcher.QColor.None);
            
            if(!ps) return;
            
            ParticleSystem.ShapeModule psShape = ps.shape;
            psShape.radius = (spriteRenderer.bounds.size.x - 2f) / 2f;
            ParticleSystem.EmissionModule module = ps.emission;
            module.rateOverTime = psShape.radius * 17f;
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
