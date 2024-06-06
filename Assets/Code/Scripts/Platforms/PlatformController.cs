using Code.Scripts.Colors;
using UnityEngine;

namespace Code.Scripts.Platforms
{
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ColorSwitcher.QColor qColor;
        
        private static readonly int On = Animator.StringToHash("On");

        private void Start()
        {
            ToggleColor(ColorSwitcher.QColor.None);
        }

        private void OnEnable()
        {
            ColorSwitcher.ColorChanged += ToggleColor;
        }

        private void ToggleColor(ColorSwitcher.QColor obj)
        {
            if (obj == qColor || qColor == ColorSwitcher.QColor.None)
                Activate();
            else
                Deactivate();
        }

        private void Activate()
        {
            animator.SetBool(On, true);
        }

        private void Deactivate()
        {
            animator.SetBool(On, false);
        }
    }
}
