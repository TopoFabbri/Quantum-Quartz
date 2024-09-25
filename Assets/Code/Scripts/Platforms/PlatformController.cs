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
        
        private static readonly int On = Animator.StringToHash("On");

        private void Start()
        {
            ToggleColor(ColorSwitcher.QColor.None);
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
