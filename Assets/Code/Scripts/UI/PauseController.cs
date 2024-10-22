using System.Collections;
using Code.Scripts.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.Scripts.UI
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseCanvas;
        [SerializeField] private Button pauseResumeButton;
    
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private string defaultMap = "World";
        [SerializeField] private string uiMap = "UI";

        private bool isPaused;

        private void OnEnable()
        {
            InputManager.Pause += Pause;
        }
    
        private void OnDisable()
        {
            InputManager.Pause -= Pause;

            Time.timeScale = 1f;
        }

        public void Pause()
        {
            isPaused = !isPaused;
        
            if (isPaused)
            {
                pauseCanvas.SetActive(true);
                pauseResumeButton.Select();
                playerInput.SwitchCurrentActionMap(uiMap);

                StartCoroutine(PauseTimeScale(0.9f));
            }
            else
            {
                Time.timeScale = 1;
                pauseCanvas.SetActive(false);
                
                playerInput.SwitchCurrentActionMap(defaultMap);
            }
        }
        
        private IEnumerator PauseTimeScale(float time)
        {
            yield return new WaitForSeconds(time);
            Time.timeScale = 0;
        }
    }
}
