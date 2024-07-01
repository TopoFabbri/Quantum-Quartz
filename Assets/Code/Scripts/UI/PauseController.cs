using Code.Scripts.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.UI
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseCanvas;
    
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
                Time.timeScale = 0;
                pauseCanvas.SetActive(true);
                
                playerInput.SwitchCurrentActionMap(uiMap);
            }
            else
            {
                Time.timeScale = 1;
                pauseCanvas.SetActive(false);
                
                playerInput.SwitchCurrentActionMap(defaultMap);
            }
        }
    }
}
