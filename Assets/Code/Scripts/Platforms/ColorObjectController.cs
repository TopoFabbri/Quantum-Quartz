using Code.Scripts.Colors;
using UnityEngine;

namespace Code.Scripts.Platforms
{
    public class ColorObjectController : MonoBehaviour
    {
        [SerializeField] private ColorSwitcher.QColor qColor;
        [SerializeField] private Animator animator;
        [SerializeField] private string animatorOnParameterName = "On";

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
        /// Toggle object on and off depending on color
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
            animator.SetBool(animatorOnParameterName, true);
        }

        /// <summary>
        /// toggle platform off
        /// </summary>
        private void Deactivate()
        {
            animator.SetBool(animatorOnParameterName, false);
        }
    }
}