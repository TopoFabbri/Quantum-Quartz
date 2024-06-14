using TMPro;
using UnityEngine;

namespace Code.Scripts.Screen
{
    public class ScreenFXController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI levelTxt;
        
        private static readonly int Ended = Animator.StringToHash("Ended");

        private void OnEnable()
        {
            LevelChanger.LevelEnd += End;
        }
        
        private void OnDisable()
        {
            LevelChanger.LevelEnd -= End;
        }

        private void Start()
        {
            levelTxt.text = "Level " + (LevelChanger.Instance.CurrentLevel + 1);
        }

        private void End()
        {
            animator.SetBool(Ended, true);
        }
        
        public void EndFadeOut()
        {
            LevelChanger.Instance.LoadNextLevel();
        }
    }
}
