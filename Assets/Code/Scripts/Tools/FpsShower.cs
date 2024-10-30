using System.Collections;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Tools
{
    public class FpsShower : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private float showInterval;

        private int fps;
        private bool updatedText = true;

        private void Update()
        {
            fps = (int) (1f / Time.deltaTime);

            if (fpsText && updatedText)
                StartCoroutine(WaitAndShowText(showInterval));
        }
        
        private IEnumerator WaitAndShowText(float time)
        {
            updatedText = false;
            yield return new WaitForSeconds(time);

            fpsText.text = fps.ToString();
            updatedText = true;
        }
    }
}
