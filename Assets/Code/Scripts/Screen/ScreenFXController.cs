using Code.Scripts.Colors;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Screen
{
    /// <summary>
    /// Manage screen effects
    /// </summary>
    public class ScreenFXController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI levelTxt;

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
            ColorSwitcher.ColorChanged += OnSwitchColorHandler;
        }

        private void Start()
        {
            levelTxt.text = "Level " + (LevelChanger.Instance.CurrentLevel + 1);
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
        /// <param name="color">New color</param>
        private void OnSwitchColorHandler(ColorSwitcher.QColor color)
        {
            animator.SetBool(SwitchedColor, true);
        }
    }
}