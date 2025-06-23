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
        [SerializeField] private TextMeshProUGUI pauseTimerText;
        [SerializeField] private TextMeshProUGUI deathsText;
        [SerializeField] private GameObject endLevelCanvas;
        [SerializeField] private GameObject optionsCanvas;

        [SerializeField] private InputManager inputManager;
        [SerializeField] private Event pauseEvent;
        [SerializeField] private Event unPauseEvent;

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

            if (!endLevelCanvas.gameObject.activeSelf)
                isPaused = !isPaused;

            if (isPaused)
            {
                inputManager.EnableUIMap();

                pauseTimerText.text = GameManager.Instance.GetTimerText().text;
                GameManager.Instance.GetTimerText().gameObject.SetActive(false);
                deathsText.text = Stats.Deaths.ToString();

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
                GameManager.Instance.GetTimerText().gameObject.SetActive(true);

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
    }
}