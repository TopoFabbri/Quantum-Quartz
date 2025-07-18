using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Menu;
using Code.Scripts.Player;
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
        [SerializeField] private bool isTimerOn;

        [SerializeField] private DroneController drone;
        [SerializeField] private PlayerController player;

        private int CurrentLevel => LevelChanger.Instance.CurrentLevel;
        public DroneController Drone => drone;
        public PlayerController Player => player;

        private void Start()
        {
#if INPUT_LAG
            Application.targetFrameRate = 20;
#else
            Application.targetFrameRate = 60;
#endif
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isTimerOn = PlayerPrefs.GetInt("Timer", 1) == 1;

            if (isTimerOn)
            {
                TimeCounter.Start();
                TimeCounter.Reset();
                Stats.SetDeaths(0);
            }
            
            if (timerTxt != null)
                timerTxt.gameObject.SetActive(isTimerOn);


            SfxController.MusicOnOff(true, gameObject);
        }

        private void OnDestroy()
        {
            SfxController.StopAllOn(gameObject);
        }

        private void OnEnable()
        {
            InputManager.Restart += OnRestartHandler;
            InputManager.DevMode += OnDevModeHandler;
            OptionsController.OnToggleTimer += HandleTimerVisibility;

        }

        private void OnDisable()
        {
            InputManager.Restart -= OnRestartHandler;
            InputManager.DevMode -= OnDevModeHandler;
            OptionsController.OnToggleTimer -= HandleTimerVisibility;
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
        
        private void HandleTimerVisibility(bool show)
        {
            if (timerTxt != null)
                timerTxt.gameObject.SetActive(show);
        }

    }
}