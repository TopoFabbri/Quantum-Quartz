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

        public enum QColor
        {
            None,
            Red,
            Blue,
            Green,
            Yellow
        }

        public QColor CurrentColor { get; private set; }

        public static event Action<QColor> ColorChanged;

        private void Start()
        {
            SetColor(QColor.None);
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
                SetColor(CurrentColor == QColor.Red ? QColor.None : QColor.Red);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color 2 is pressed
        /// </summary>
        private void OnColor2()
        {
            if (blue)
                SetColor(CurrentColor == QColor.Blue ? QColor.None : QColor.Blue);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color 3 is pressed
        /// </summary>
        private void OnColor3()
        {
            if (green)
                SetColor(CurrentColor == QColor.Green ? QColor.None : QColor.Green);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Handle when color 4 is pressed
        /// </summary>
        private void OnColor4()
        {
            if (yellow)
                SetColor(CurrentColor == QColor.Yellow ? QColor.None : QColor.Yellow);
            else
                SfxController.BlockedCrystal(gameObject);
        }

        /// <summary>
        /// Change to new color
        /// </summary>
        /// <param name="color"></param>
        private void SetColor(QColor color)
        {
            ColorChanged?.Invoke(color);

            CurrentColor = color;
        }
    }
}