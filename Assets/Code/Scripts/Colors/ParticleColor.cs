using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Colors
{
    public class ParticleColor : MonoBehaviour
    {
        [SerializeField] private List<Color> colors = new();
        [SerializeField] private Material particleMaterial;

        private void Awake()
        {
            if (colors.Count <= 0) return;
            
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
            particleMaterial.color = Color.white;
        }

        private void OnColorSwitch(ColorSwitcher.QColor color)
        {
            particleMaterial.color = colors[(int) color];
        }
    }
}