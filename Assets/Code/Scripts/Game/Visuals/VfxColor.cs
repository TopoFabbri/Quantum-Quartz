using Code.Scripts.Game.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Scripts.Game.Visuals
{
    public class VfxColor : MonoBehaviour
    {
        [Header("Settings")] [SerializeField]
        private bool changeExistingParticles;

        [Header("Vfx References")]
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
                ParticleSystem.MainModule mainPs = ps.main;
                
                if (changeExistingParticles)
                {
                    int max = mainPs.maxParticles > 0 ? mainPs.maxParticles : 1024;
                    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[max];
                    int alive = ps.GetParticles(particles);
                    
                    for (int i = 0; i < alive; i++)
                    {
                        ParticleSystem.Particle p = particles[i];
                        p.startColor = col;
                        particles[i] = p;
                    }
                    if (alive > 0)
                    {
                        ps.SetParticles(particles, alive);
                    }
                }
                
                mainPs.startColor = col;
            }
            foreach (Light2D light in lights)
            {
                light.color = col;
            }
        }
    }
}