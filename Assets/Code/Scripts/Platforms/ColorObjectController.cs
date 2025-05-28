using System;
using System.Collections.Generic;
using Code.Scripts.Colors;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Platforms
{
    public class ColorObjectController : MonoBehaviour
    {
        [FormerlySerializedAs("qColor")] [SerializeField] private ColorSwitcher.QColour qColour;
        [SerializeField] private Animator animator;
        [SerializeField] private string animatorOnParameterName = "On";
        [SerializeField] private List<Behaviour> objectsToToggle = new();
        
        public event Action<bool> Toggled;
        
        private void Start()
        {
            ToggleColor(ColorSwitcher.QColour.None);
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
        /// <param name="colour">New world color</param>
        private void ToggleColor(ColorSwitcher.QColour colour)
        {
            if (colour == qColour || qColour == ColorSwitcher.QColour.None)
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
            Toggled?.Invoke(true);
        }

        /// <summary>
        /// toggle platform off
        /// </summary>
        private void Deactivate()
        {
            animator.SetBool(animatorOnParameterName, false);
            Toggled?.Invoke(false);
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