using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Code.Scripts.Colors
{
    public class VfxColor : MonoBehaviour
    {
        [SerializeField] private List<Color> colors = new List<Color>() { Color.white, Color.red, Color.blue, Color.green, Color.yellow };
        [SerializeField] private List<ParticleSystem> particleSystems;
        [SerializeField] private List<Light2D> lights;

        private void OnEnable()
        {
            ColorSwitcher.ColorChanged += OnColorSwitch;
            OnColorSwitch(ColorSwitcher.Instance.CurrentColor);
        }

        private void OnDisable()
        {
            ColorSwitcher.ColorChanged -= OnColorSwitch;
        }

        private void OnColorSwitch(ColorSwitcher.QColor color)
        {
            Color col = (int)color >= colors.Count ? Color.clear : colors[(int)color];
            foreach (ParticleSystem ps in particleSystems)
            {
                var mainPs = ps.main;
                mainPs.startColor = col;
            }
            foreach (Light2D light in lights)
            {
                light.color = col;
            }
        }
    }
}