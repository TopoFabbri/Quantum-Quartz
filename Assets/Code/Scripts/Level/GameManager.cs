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

        private int CurrentLevel => LevelChanger.Instance.CurrentLevel;

        private void Start()
        {
            Application.targetFrameRate = 60;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isTimerOn = PlayerPrefs.GetInt("Timer", 1) == 1;

            if (isTimerOn)
            {
                TimeCounter.Start();
                TimeCounter.Reset();
                Stats.SetDeaths(0);
            }

            SfxController.MusicOnOff(true, gameObject);
        }

        private void OnDestroy()
        {
            SfxController.MusicOnOff(false, gameObject);
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
        }
        
        /// <summary>
        /// Get Timer text
        /// </summary>
        public TextMeshProUGUI GetTimerText()
        {
            return timerTxt;
        }

        public Timer GetLevelTime()
        {
            return Stats.GetLevelTime(CurrentLevel);
        }

        public void PickUpCollectible(int id)
        {
            Stats.PickUpCollectible(CurrentLevel, id);
        }

        public bool HasCollectible(int id)
        {
            return Stats.HasCollectible(CurrentLevel, id);
        }
    }
}