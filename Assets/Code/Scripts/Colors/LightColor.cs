using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Scripts.Colors
{
    public class LightColor : MonoBehaviour
    {
        [SerializeField] private List<Color> colors = new();

        private new Light2D light;

        private void Awake()
        {
            light = GetComponent<Light2D>();
            
            if (!light)
                light = gameObject.AddComponent<Light2D>();
            
            if (colors.Count > 0)
                return;
            
            colors.Add(Color.white);
            colors.Add(Color.red);
            colors.Add(Color.blue);
            colors.Add(Color.green);
            colors.Add(Color.yellow);
        }

        private void OnEnable()
        {
            ColorSwitcher.ColorChanged += OnColorSwitch;
        }
        
        private void OnDisable()
        {
            ColorSwitcher.ColorChanged -= OnColorSwitch;
        }

        private void OnColorSwitch(ColorSwitcher.QColor colour)
        {
            light.color = colors[(int) colour];
        }
    }
}