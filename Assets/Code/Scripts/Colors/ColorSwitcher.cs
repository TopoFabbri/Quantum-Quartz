using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Colors
{
    /// <summary>
    /// Manage colors
    /// </summary>
    public class ColorSwitcher : MonoBehaviourSingleton<ColorSwitcher>
    {
        [SerializeField] private bool blue = true;
        [SerializeField] private bool red;
        [SerializeField] private bool green;
        [SerializeField] private bool yellow;

        public enum QColor
        {
            None = 0,
            Red = 1 << 0,
            Blue = 1 << 1,
            Green = 1 << 2,
            Yellow = 1 << 3
        }

        public QColor CurrentColor { get; private set; }
        public IReadOnlyList<QColor> EnabledColors { get; private set; }

        public static event Action<QColor> ColorChanged;

        private QColor? preLockedColor = null;

        private void Start()
        {
            SetColor(QColor.None);
            UpdateEnabledColors();
        }

        private void OnEnable()
        {
            InputManager.ColorRed += OnColorRed;
            InputManager.ColorBlue += OnColorBlue;
            InputManager.ColorGreen += OnColorGreen;
            InputManager.ColorYellow += OnColorYellow;
        }

        private void OnDisable()
        {
            InputManager.ColorRed -= OnColorRed;
            InputManager.ColorBlue -= OnColorBlue;
            InputManager.ColorGreen -= OnColorGreen;
            InputManager.ColorYellow -= OnColorYellow;
        }

        private void OnValidate()
        {
            UpdateEnabledColors();
        }

        private void UpdateEnabledColors()
        {
            List<QColor> temp = new List<QColor>();
            if (blue)   temp.Add(QColor.Blue);
            if (red)    temp.Add(QColor.Red);
            if (green)  temp.Add(QColor.Green);
            if (yellow) temp.Add(QColor.Yellow);
            EnabledColors = temp;
        }

        /// <summary>
        /// Handle when color red is pressed
        /// </summary>
        private void OnColorRed()
        {
            if (red && !preLockedColor.HasValue)
            {
                SetColor(QColor.Red);
            }
            else
            {
                SfxController.BlockedCrystal(gameObject);
            }
        }

        /// <summary>
        /// Handle when color blue is pressed
        /// </summary>
        private void OnColorBlue()
        {
            if (blue && !preLockedColor.HasValue)
            {
                SetColor(QColor.Blue);
            }
            else
            {
                SfxController.BlockedCrystal(gameObject);
            }
        }

        /// <summary>
        /// Handle when color green is pressed
        /// </summary>
        private void OnColorGreen()
        {
            if (green && !preLockedColor.HasValue)
            {
                SetColor(QColor.Green);
            }
            else
            {
                SfxController.BlockedCrystal(gameObject);
            }
        }

        /// <summary>
        /// Handle when color yellow is pressed
        /// </summary>
        private void OnColorYellow()
        {
            if (yellow && !preLockedColor.HasValue)
            {
                SetColor(QColor.Yellow);
            }
            else
            {
                SfxController.BlockedCrystal(gameObject);
            }
        }

        /// <summary>
        /// Change to new color
        /// </summary>
        /// <param name="color"></param>
        private void SetColor(QColor color)
        {
            if (CurrentColor != color)
            {
                ColorChanged?.Invoke(color);

                CurrentColor = color;

                SfxController.ChangeToCrystal(gameObject, color);
            }
        }

        public void LockColor(QColor color) {
            if (preLockedColor == null)
            {
                preLockedColor = CurrentColor;
            }

            SetColor(color);
        }

        public void UnlockColor(QColor color)
        {
            if (preLockedColor.HasValue && CurrentColor == color)
            {
                if (!EnabledColors.Contains(color))
                {
                    SetColor(preLockedColor.Value);
                }
                preLockedColor = null;
            }
        }

        public void EnableColor(QColor color)
        {
            switch (color)
            {
                case QColor.None:
                    break;
                case QColor.Blue:
                    blue = true;
                    break;
                case QColor.Red:
                    red = true;
                    break;
                case QColor.Green:
                    green  = true;
                    break;
                case QColor.Yellow:
                    yellow  = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
            Stats.PickUpColor(color);
            UpdateEnabledColors();
        }
    }
}