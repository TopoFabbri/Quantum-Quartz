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
        
        private bool ended;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            TimeCounter.Start();
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
            timerTxt.text = TimeCounter.Time;
        }

        /// <summary>
        /// Set dev mode
        /// </summary>
        private void OnDevModeHandler()
        {
            Settings.devMode = !Settings.devMode;
            
            statesText.SetActive(Settings.devMode);
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
