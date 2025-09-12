using System;
using System.Collections.Generic;
using Code.Scripts.Colors;
using Code.Scripts.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Platforms
{
    public class ColorObjectController : MonoBehaviour
    {
        public event Action<bool> Toggled;

        [FormerlySerializedAs("qColour"), SerializeField] private ColorSwitcher.QColor qColor;
        [SerializeField] private bool hasActivationCollision = true;
        [SerializeField] private Animator animator;
        [SerializeField] private string animatorOnParameterName = "On";
        [SerializeField] private List<Behaviour> objectsToToggle = new();

        private static ContactFilter2D playerFilter;
        private Collider2D col;

        private void Start()
        {
            playerFilter = new ContactFilter2D { layerMask = LayerMask.GetMask("Player") };
            if (qColor != ColorSwitcher.QColor.None && ColorSwitcher.Instance.CurrentColor != qColor)
            {
                Deactivate();
            }
            col = GetComponent<Collider2D>();
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
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        /// <summary>
        /// Toggle platform on
        /// </summary>
        private void Activate()
        {
            animator.SetBool(animatorOnParameterName, true);
            Toggled?.Invoke(true);

            if (!hasActivationCollision)
            {
                List<Collider2D> hits = new List<Collider2D>();
                bool temp = col.enabled;
                col.enabled = true;
                if (col.OverlapCollider(playerFilter, hits) > 0)
                {
                    foreach (Collider2D hit in hits)
                    {
                        if (!hit.isTrigger && hit.gameObject.CompareTag("Player") && hit.TryGetComponent(out PlayerController _))
                        {
                            col.isTrigger = true;
                            break;
                        }
                    }
                }
                col.enabled = temp;
            }
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
            {
                component.enabled = !component.enabled;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!hasActivationCollision && !other.isTrigger && other.gameObject.CompareTag("Player") && other.TryGetComponent(out PlayerController _))
            {
                col.isTrigger = false;
            }
        }
    }
}