using Code.Scripts.Colors;
using Code.Scripts.Level;
using UnityEngine;

namespace Code.Scripts.Screen
{
    /// <summary>
    /// Manage screen effects
    /// </summary>
    public class ScreenFXController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int Ended = Animator.StringToHash("Ended");
        private static readonly int SwitchedColor = Animator.StringToHash("SwitchedColor");

        private void OnEnable()
        {
            LevelChanger.LevelEnd += End;
            ColorSwitcher.ColorChanged += OnSwitchColorHandler;
        }

        private void OnDisable()
        {
            LevelChanger.LevelEnd -= End;
            ColorSwitcher.ColorChanged -= OnSwitchColorHandler;
        }

        /// <summary>
        /// Start end animation
        /// </summary>
        private void End()
        {
            animator.SetBool(Ended, true);
        }

        /// <summary>
        /// Change level on end fade out
        /// </summary>
        public void EndFadeOut()
        {
            LevelChanger.Instance.LoadNextLevel();
        }

        /// <summary>
        /// Stop flash
        /// </summary>
        public void EndFlash()
        {
            animator.SetBool(SwitchedColor, false);
        }
        
        /// <summary>
        /// Handle color switched
        /// </summary>
        /// <param name="colour">New color</param>
        private void OnSwitchColorHandler(ColorSwitcher.QColor colour)
        {
            animator.SetBool(SwitchedColor, true);
        }
    }
}