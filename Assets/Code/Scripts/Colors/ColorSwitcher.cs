using System;
using Code.Scripts.Input;
using Code.Scripts.Tools;

namespace Code.Scripts.Colors
{
    public class ColorSwitcher : MonoBehaviourSingleton<ColorSwitcher>
    {
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
           SetColor(CurrentColor == QColor.Red ? QColor.None : QColor.Red);
        }

        private void OnColor2()
        {
            SetColor(CurrentColor == QColor.Blue ? QColor.None : QColor.Blue);
        }

        private void OnColor3()
        {
            SetColor(CurrentColor == QColor.Green ? QColor.None : QColor.Green);
        }

        private void OnColor4()
        {
            SetColor(CurrentColor == QColor.Yellow ? QColor.None : QColor.Yellow);
        }

        private void SetColor(QColor color)
        {
            ColorChanged?.Invoke(color);
            
            CurrentColor = color;
        }
    }
}