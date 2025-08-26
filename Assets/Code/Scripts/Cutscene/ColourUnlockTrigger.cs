using System.Collections.Generic;
using Code.Scripts.Colors;
using UnityEngine;

namespace Code.Scripts.Cutscene
{
    public class ColourUnlockTrigger : MonoBehaviour
    {
        [SerializeField] private List<ColorSwitcher.QColor> coloursToUnlock;

        private void OnTriggerEnter2D(Collider2D other)
        {
            foreach (ColorSwitcher.QColor qColor in coloursToUnlock)
                ColorSwitcher.Instance.EnableColor(qColor);
        }
    }
}