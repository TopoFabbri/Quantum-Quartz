using Code.Scripts.Game.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Game.Triggers
{
    public class ColorUnlockTrigger : InteractableComponent
    {
        [SerializeField] private List<ColorSwitcher.QColor> colorsToUnlock;

        protected override void OnInteracted()
        {
            foreach (ColorSwitcher.QColor color in colorsToUnlock)
            {
                ColorSwitcher.Instance.EnableColor(color);
            }
        }
    }
}