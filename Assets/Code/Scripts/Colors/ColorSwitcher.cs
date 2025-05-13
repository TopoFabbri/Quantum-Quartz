using System;
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

        public static event Action<QColour> ColorChanged;

        private void Start()
        {
            SetColor(QColour.None);
        }

        private void OnEnable()
        {
            InputManager.Color1 += OnColor1;
            InputManager.Color2 += OnColor2;
            InputManager.Color3 += OnColor3;
            InputManager.Color4 += OnColor4;
        }

        private void OnDisable()
        {
            InputManager.Color1 -= OnColor1;
            InputManager.Color2 -= OnColor2;
            InputManager.Color3 -= OnColor3;
            InputManager.Color4 -= OnColor4;
        }

        /// <summary>
        /// Handle when color 1 is pressed
        /// </summary>
        private void OnColor1()
        {
            if (red)
                SetColor(CurrentColour == QColour.Red ? QColour.None : QColour.Red);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color 2 is pressed
        /// </summary>
        private void OnColor2()
        {
            if (blue)
                SetColor(CurrentColour == QColour.Blue ? QColour.None : QColour.Blue);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color 3 is pressed
        /// </summary>
        private void OnColor3()
        {
            if (green)
                SetColor(CurrentColour == QColour.Green ? QColour.None : QColour.Green);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color 4 is pressed
        /// </summary>
        private void OnColor4()
        {
            if (yellow)
                SetColor(CurrentColour == QColour.Yellow ? QColour.None : QColour.Yellow);
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