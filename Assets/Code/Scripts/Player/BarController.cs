using System.Collections.Generic;
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
        [SerializeField] private GameObject barObject;
        
        private readonly Dictionary<ColorSwitcher.QColor, StaminaBar> barsByColors = new();
        
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
            if (!barsByColors.TryGetValue(ColorSwitcher.Instance.CurrentColor, out StaminaBar staminaBar))
            {
                barObject.SetActive(false);
                return;
            }
            
            barObject.SetActive(!staminaBar.Full);
            fill.fillAmount = staminaBar.FillValue - staminaBar.FillValue % strides;
        }

        private void LateUpdate()
        {
            foreach (KeyValuePair<ColorSwitcher.QColor, StaminaBar> barByColor in barsByColors)
                barByColor.Value.LateUpdate();
        }

        /// <summary>
        /// Retrieves the stamina bar associated with the specified colour.
        /// </summary>
        /// <param name="colour">The colour of the stamina bar to retrieve</param>
        /// <returns>The StaminaBar instance associated with the specified colour, or null if not found</returns>
        public StaminaBar GetBar(ColorSwitcher.QColor colour)
        {
            return barsByColors.GetValueOrDefault(colour);
        }
        
        /// <summary>
        /// Adds a new stamina bar for a specific colour to the controller.
        /// </summary>
        /// <param name="colour">The colour associated with the stamina bar</param>
        /// <param name="regenSpeed">The regeneration speed of the stamina bar</param>
        /// <param name="depleteSpeed">The depletion speed of the stamina bar</param>
        public void AddBar(ColorSwitcher.QColor colour, float regenSpeed, float depleteSpeed, float initCut)
        {
            barsByColors.Add(colour, new StaminaBar(regenSpeed, depleteSpeed, initCut));
        }
        
        /// <summary>
        /// Change bar colour and reset
        /// </summary>
        /// <param name="colour"></param>
        private void SwitchColor(ColorSwitcher.QColor colour)
        {
            fill.color = colour switch
            {
                ColorSwitcher.QColor.Green => Color.green,
                ColorSwitcher.QColor.Yellow => Color.yellow,
                ColorSwitcher.QColor.Red => Color.red,
                _ => fill.color
            };
        }
    }
}