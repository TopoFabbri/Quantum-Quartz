using System;
using Code.Scripts.Input;
using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Colors
{
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

        private void OnColor1()
        {
            if (red)
               SetColor(CurrentColor == QColor.Red ? QColor.None : QColor.Red);
        }

        private void OnColor2()
        {
            if (blue)
                SetColor(CurrentColor == QColor.Blue ? QColor.None : QColor.Blue);
        }

        private void OnColor3()
        {
            if (green)
                SetColor(CurrentColor == QColor.Green ? QColor.None : QColor.Green);
        }

        private void OnColor4()
        {
            if (yellow)
                SetColor(CurrentColor == QColor.Yellow ? QColor.None : QColor.Yellow);
        }

        private void SetColor(QColor color)
        {
            ColorChanged?.Invoke(color);
            
            CurrentColor = color;
        }
    }
}