using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Colors
{
    public class ParticleColor : MonoBehaviour
    {
        [FormerlySerializedAs("colors")] [SerializeField] private List<Color> colours = new();
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private ColorSwitcher.QColour qColour;
        
        private ParticleSystem.MainModule mainPs;

        private bool on;

        private void Awake()
        {
            mainPs = ps.main;
            if (colours.Count > 0) return;

            colours.Add(Color.white);
            colours.Add(Color.red);
            colours.Add(Color.blue);
            colours.Add(Color.green);
            colours.Add(Color.yellow);
        }

        private void OnEnable()
        {
            ColorSwitcher.ColorChanged += OnColorSwitch;
        }

        private void OnDisable()
        {
            ColorSwitcher.ColorChanged -= OnColorSwitch;
        }

        private void OnColorSwitch(ColorSwitcher.QColour colour)
        {
            mainPs.startColor = colours[(int)colour];
        }
    }
}