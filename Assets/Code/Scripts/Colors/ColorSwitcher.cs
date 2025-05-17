using System;
using System.Collections.Generic;
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

        public enum QColour
        {
            None,
            Red,
            Blue,
            Green,
            Yellow
        }

        public QColour CurrentColour { get; private set; }
        public IReadOnlyList<QColour> EnabledColours { get; private set; }

        public static event Action<QColour> ColorChanged;

        private void Start()
        {
            SetColor(QColour.None);
            UpdateEnabledColours();
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
            UpdateEnabledColours();
        }

        private void UpdateEnabledColours()
        {
            List<QColour> temp = new List<QColour>();
            if (blue)
                temp.Add(QColour.Blue);
            if (red)
                temp.Add(QColour.Red);
            if (green)
                temp.Add(QColour.Green);
            if (yellow)
                temp.Add(QColour.Yellow);
            EnabledColours = temp;
        }

        /// <summary>
        /// Handle when color red is pressed
        /// </summary>
        private void OnColorRed()
        {
            if (red)
                SetColor(QColour.Red);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color blue is pressed
        /// </summary>
        private void OnColorBlue()
        {
            if (blue)
                SetColor(QColour.Blue);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color green is pressed
        /// </summary>
        private void OnColorGreen()
        {
            if (green)
                SetColor(QColour.Green);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color yellow is pressed
        /// </summary>
        private void OnColorYellow()
        {
            if (yellow)
                SetColor(QColour.Yellow);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Change to new color
        /// </summary>
        /// <param name="colour"></param>
        private void SetColor(QColour colour)
        {
            ColorChanged?.Invoke(colour);

            CurrentColour = colour;
            
            SfxController.ChangeToCrystal(gameObject, colour);
        }
    }
}