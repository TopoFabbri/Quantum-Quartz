using Code.Scripts.Colors;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Control bar actions
    /// </summary>
    public class BarController : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private float strides = float.Epsilon;

        public float FillValue
        {
            get => fillValue;
            set => fillValue = Mathf.Clamp01(value);
        }

    public bool depleted;

        private float fillValue = 1f;
        
        private void Start()
        {
            ColorSwitcher.ColorChanged += SwitchColor;
        }
        
        private void OnDestroy()
        {
            ColorSwitcher.ColorChanged -= SwitchColor;
        }
        
        private void Update()
        {
            if (FillValue <= 0f)
                depleted = true;
            else if (depleted && FillValue >= 1f)
                depleted = false;
            
            SetVisibility(FillValue < 1f);
            
            fill.fillAmount = FillValue - FillValue % strides;
        }
        
        /// <summary>
        /// Set bar visibility
        /// </summary>
        /// <param name="visible"> Visibility value</param>
        public void SetVisibility(bool visible)
        {
            if (depleted)
            {
                gameObject.SetActive(true);
                return;
            }
            
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// Reset bar value
        /// </summary>
        public void Reset()
        {
            FillValue = 1f;
        }

        /// <summary>
        /// Change bar color and reset
        /// </summary>
        /// <param name="color"></param>
        private void SwitchColor(ColorSwitcher.QColor color)
        {
            Reset();
            SetVisibility(false);

            fill.color = color switch
            {
                ColorSwitcher.QColor.Green => Color.green,
                ColorSwitcher.QColor.Yellow => Color.yellow,
                _ => fill.color
            };
        }
    }
}
