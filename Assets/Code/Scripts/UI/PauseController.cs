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
        [SerializeField] private string pauseEvent = "Set_State_Music_Pause";
        [SerializeField] private string unPauseEvent = "Set_State_Music_Ingame";

        private bool isPaused;
        private bool inCoroutine;

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
            if (inCoroutine) return;
            
            isPaused = !isPaused;
        
            if (isPaused)
            {
                pauseCanvas.SetActive(true);
                pauseResumeButton.Select();
                playerInput.SwitchCurrentActionMap(uiMap);
                AkSoundEngine.PostEvent(pauseEvent, gameObject);
                
                StartCoroutine(PauseTimeScale(0.9f));
            }
            else
            {
                Time.timeScale = 1;
                pauseCanvas.SetActive(false);
                AkSoundEngine.PostEvent(unPauseEvent, gameObject);
                
                playerInput.SwitchCurrentActionMap(defaultMap);
            }
        }
        
        private IEnumerator PauseTimeScale(float time)
        {
            inCoroutine = true;
            yield return new WaitForSeconds(time);
            
            Time.timeScale = 0;
            inCoroutine = false;
        }

        private void OnDestroy()
        {
            AkSoundEngine.PostEvent(unPauseEvent, gameObject);
        }
    }
}
