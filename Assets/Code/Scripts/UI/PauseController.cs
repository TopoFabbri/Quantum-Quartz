using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = AK.Wwise.Event;

namespace Code.Scripts.UI
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseCanvas;
        [SerializeField] private Button pauseResumeButton;
        [SerializeField] private GameObject gameHUD;
        [SerializeField] private TextMeshProUGUI gameTimerText;
        [SerializeField] private TextMeshProUGUI pauseTimerText;
        [SerializeField] private TextMeshProUGUI deathsText;
        [SerializeField] private GameObject endLevelCanvas;
        [SerializeField] private GameObject optionsCanvas;

        [SerializeField] private InputManager inputManager;
        [SerializeField] private Event pauseEvent;
        [SerializeField] private Event unPauseEvent;

        private bool isPaused = false;
        private bool inCoroutine = false;
        private bool inDialogue = false;

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
            if (inCoroutine || inDialogue)
                return;

            if (!endLevelCanvas.gameObject.activeSelf)
                isPaused = !isPaused;

            if (isPaused)
            {
                inputManager.EnableUIMap();

                pauseTimerText.text = TimeCounter.Time.ToStr;
                gameHUD.SetActive(false);
                gameTimerText.gameObject.SetActive(false);
                deathsText.text = Stats.GetDeaths().ToString();

                pauseCanvas.SetActive(true);
                pauseResumeButton.Select();
                pauseEvent.Post(gameObject);

                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                pauseCanvas.SetActive(false);
                optionsCanvas.SetActive(false);
                unPauseEvent.Post(gameObject);
                gameHUD.SetActive(true);
                gameTimerText.gameObject.SetActive(Settings.ShowGameTimer);

                inputManager.EnableGameMap();
            }
        }

        private void OnDestroy()
        {
            unPauseEvent.Post(gameObject);
        }
        
        private void OnUIBack()
        {
            Pause();
        }

        public void EnterDialogue()
        {
            inDialogue = true;
        }

        public void ExitDialogue()
        {
            inDialogue = false;
        }
    }
}