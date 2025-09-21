using Code.Scripts.Game.Managers;
using System;
using UnityEngine;

namespace Code.Scripts.Game.Visuals
{
    public class VolumeAnimManager : MonoBehaviour
    {
        [SerializeField] private Animator animController;
        [SerializeField] private string blueTrigger = "BlueTrigger";
        [SerializeField] private string redTrigger = "RedTrigger";
        [SerializeField] private string greenTrigger = "GreenTrigger";
        [SerializeField] private string yellowTrigger = "YellowTrigger";

        private void OnEnable()
        {
            ColorSwitcher.ColorChanged += OnColorChanged;
        }
        
        private void OnDisable()
        {
            ColorSwitcher.ColorChanged -= OnColorChanged;
        }
        

        private void OnColorChanged(ColorSwitcher.QColor obj)
        {
            switch (obj)
            {
                case ColorSwitcher.QColor.None:
                    break;
                
                case ColorSwitcher.QColor.Red:
                    animController.SetTrigger(redTrigger);
                    break;
                
                case ColorSwitcher.QColor.Blue:
                    animController.SetTrigger(blueTrigger);
                    break;
                
                case ColorSwitcher.QColor.Green:
                    animController.SetTrigger(greenTrigger);
                    break;
                
                case ColorSwitcher.QColor.Yellow:
                    animController.SetTrigger(yellowTrigger);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }
    }
}
