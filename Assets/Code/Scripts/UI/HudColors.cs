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
            OnColorChangedHandler(ColorSwitcher.Instance.CurrentColour);
        }

        private void OnEnable()
        {
            ColorSwitcher.ColorChanged += OnColorChangedHandler;
        }

        private void OnDisable()
        {
            ColorSwitcher.ColorChanged -= OnColorChangedHandler;
        }

        private void OnColorChangedHandler(ColorSwitcher.QColour colour)
        {
            ToggleColor(blueButton, colour == ColorSwitcher.QColour.Blue);
            ToggleColor(redButton, colour == ColorSwitcher.QColour.Red);
            ToggleColor(greenButton, colour == ColorSwitcher.QColour.Green);
            ToggleColor(yellowButton, colour == ColorSwitcher.QColour.Yellow);

            curAbTxt.text = colour switch
            {
                ColorSwitcher.QColour.None or ColorSwitcher.QColour.Green or ColorSwitcher.QColour.Yellow => "None",
                ColorSwitcher.QColour.Red => "Dash",
                ColorSwitcher.QColour.Blue => "Djmp",
                _ => throw new ArgumentOutOfRangeException(nameof(colour), colour, null)
            };
        }

        private void ToggleColor(Image color, bool isOn)
        {
            color.color = new Color(color.color.r, color.color.g, color.color.b, isOn ? 1f : offAlpha);
        }
    }
}