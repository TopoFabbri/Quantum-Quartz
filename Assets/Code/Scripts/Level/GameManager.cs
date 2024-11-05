using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Level
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private TextMeshProUGUI timerTxt;
        [SerializeField] private GameObject statesText;
        public bool isTimerOn;

        private bool ended;

        private void Start()
        {
            Application.targetFrameRate = 60;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isTimerOn = PlayerPrefs.GetInt("Timer", 1) == 1;

            if (isTimerOn)
                TimeCounter.Start();

            SfxController.MusicOnOff(true, gameObject);
        }

        private void OnDestroy()
        {
            SfxController.MusicOnOff(false, gameObject);
            
            Stats.SetTime(TimeCounter.Time);
        }

        private void OnEnable()
        {
            InputManager.Restart += OnRestartHandler;
            InputManager.DevMode += OnDevModeHandler;
        }

        private void OnDisable()
        {
            InputManager.Restart -= OnRestartHandler;
            InputManager.DevMode -= OnDevModeHandler;
        }

        private void Update()
        {
            TimeCounter.Update(Time.deltaTime);
            timerTxt.text = TimeCounter.Time.ToStr;
        }

        /// <summary>
        /// Set dev mode
        /// </summary>
        private void OnDevModeHandler()
        {
            Settings.Instance.devMode = !Settings.Instance.devMode;

            statesText.SetActive(Settings.Instance.devMode);
            Settings.MusicVol = Settings.Instance.devMode ? 0 : 100f;
        }

        /// <summary>
        /// Restart level
        /// </summary>
        private static void OnRestartHandler()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            TimeCounter.Reset();
        }
    }
}