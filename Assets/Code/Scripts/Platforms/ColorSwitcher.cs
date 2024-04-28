using System;
using Code.Scripts.Input;
using Code.Scripts.Tools;

namespace Code.Scripts.Platforms
{
    public class ColorSwitcher : MonoBehaviourSingleton<ColorSwitcher>
    {
        public enum QColors
        {
            None,
            Red,
            Blue,
            Green,
            Yellow
        }

        public QColors CurrentColor { get; private set; }
        
        public static event Action<QColors> ColorChanged;

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
           SetColor(CurrentColor == QColors.Red ? QColors.None : QColors.Red);
        }

        private void OnColor2()
        {
            SetColor(CurrentColor == QColors.Blue ? QColors.None : QColors.Blue);
        }

        private void OnColor3()
        {
            SetColor(CurrentColor == QColors.Green ? QColors.None : QColors.Green);
        }

        private void OnColor4()
        {
            SetColor(CurrentColor == QColors.Yellow ? QColors.None : QColors.Yellow);
        }

        private void SetColor(QColors color)
        {
            ColorChanged?.Invoke(color);
            
            CurrentColor = color;
        }
    }
}