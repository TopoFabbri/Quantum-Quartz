using System;
using Code.Scripts.Colors;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Code.Scripts.UI
{
    public class HudColors : MonoBehaviour
    {
        [SerializeField] private Image blueButton;
        [SerializeField] private Image redButton;
        [SerializeField] private Image greenButton;
        [SerializeField] private Image yellowButton;
        [SerializeField] private TextMeshProUGUI curAbTxt;
        
        [SerializeField] private float offAlpha = .05f;

        private void Start()
        {
            OnColorChangedHandler(ColorSwitcher.Instance.CurrentColor);
        }

        private void OnEnable()
        {
            ColorSwitcher.ColorChanged += OnColorChangedHandler;
        }

        private void OnDisable()
        {
            ColorSwitcher.ColorChanged -= OnColorChangedHandler;
        }

        private void OnColorChangedHandler(ColorSwitcher.QColor color)
        {
            ToggleColor(blueButton, color == ColorSwitcher.QColor.Blue);
            ToggleColor(redButton, color == ColorSwitcher.QColor.Red);
            ToggleColor(greenButton, color == ColorSwitcher.QColor.Green);
            ToggleColor(yellowButton, color == ColorSwitcher.QColor.Yellow);

            curAbTxt.text = color switch
            {
                ColorSwitcher.QColor.None or ColorSwitcher.QColor.Green or ColorSwitcher.QColor.Yellow => "None",
                ColorSwitcher.QColor.Red => "Dash",
                ColorSwitcher.QColor.Blue => "Djmp",
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }

        private void ToggleColor(Image color, bool isOn)
        {
            color.color = new Color(color.color.r, color.color.g, color.color.b, isOn ? 1f : offAlpha);
        }
    }
}