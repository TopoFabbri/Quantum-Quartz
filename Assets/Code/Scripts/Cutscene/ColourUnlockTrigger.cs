using System.Collections.Generic;
using Code.Scripts.Colors;
using UnityEngine;

namespace Code.Scripts.Cutscene
{
    public class ColorUnlockTrigger : MonoBehaviour
    {
        [SerializeField] private List<ColorSwitcher.QColor> colorsToUnlock;

        private void OnTriggerEnter2D(Collider2D other)
        {
            foreach (ColorSwitcher.QColor qColor in colorsToUnlock)
                ColorSwitcher.Instance.EnableColor(qColor);
        }
    }
}