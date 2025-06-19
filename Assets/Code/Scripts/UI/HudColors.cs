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

        private void OnColorChangedHandler(ColorSwitcher.QColor colour)
        {
            ToggleColor(blueButton, colour == ColorSwitcher.QColor.Blue);
            ToggleColor(redButton, colour == ColorSwitcher.QColor.Red);
            ToggleColor(greenButton, colour == ColorSwitcher.QColor.Green);
            ToggleColor(yellowButton, colour == ColorSwitcher.QColor.Yellow);

            curAbTxt.text = colour switch
            {
                ColorSwitcher.QColor.None or ColorSwitcher.QColor.Green or ColorSwitcher.QColor.Yellow => "None",
                ColorSwitcher.QColor.Red => "Dash",
                ColorSwitcher.QColor.Blue => "Djmp",
                _ => throw new ArgumentOutOfRangeException(nameof(colour), colour, null)
            };
        }

        private void ToggleColor(Image color, bool isOn)
        {
            color.color = new Color(color.color.r, color.color.g, color.color.b, isOn ? 1f : offAlpha);
        }
    }
}