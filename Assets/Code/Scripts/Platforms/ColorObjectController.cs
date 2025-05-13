using System.Collections.Generic;
using Code.Scripts.Colors;
using UnityEngine;

namespace Code.Scripts.Platforms
{
    public class ColorObjectController : MonoBehaviour
    {
        [SerializeField] private ColorSwitcher.QColor qColor;
        [SerializeField] private Animator animator;
        [SerializeField] private string animatorOnParameterName = "On";
        [SerializeField] private List<Behaviour> objectsToToggle = new();
        
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

        /// <summary>
        /// Toggles the enabled state of all MonoBehaviour components in the objectsToToggle list.
        /// Each object's enabled state is switched to its opposite value (enabled becomes disabled and vice versa).
        /// </summary>
        public void ToggleObjects()
        {
            foreach (Behaviour component in objectsToToggle)
                component.enabled = !component.enabled;
        }
    }
}