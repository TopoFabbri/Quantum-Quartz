using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Scripts.Level
{
    public class InteractableLightController : InteractableController2D
    {
        [SerializeField] private Light2D light2D;

        [Header("BlinkSettings")] 
        [SerializeField] private int blinkQty = 3;
        [SerializeField] private float minRateRange = .1f;
        [SerializeField] private float maxRateRange = .5f;

        private bool blinking;
        
        protected override void OnInteracted()
        {
            if (blinking) return;
            
            StartCoroutine(LightBlink());
        }

        private IEnumerator LightBlink()
        {
            blinking = true;
            
            float lightIntensity = light2D.intensity;
            
            for (int i = 0; i < blinkQty; i++)
            {
                float rate = Random.Range(minRateRange, maxRateRange);

                float t = 0f;
                
                light2D.intensity = 0f;
                
                while (t < 1f)
                {
                    t += Time.deltaTime * rate;
                    light2D.intensity = Mathf.Lerp(0f, lightIntensity, t);
                    yield return null;
                }
            }

            light2D.intensity = lightIntensity;
            
            blinking = false;
        }
    }
}