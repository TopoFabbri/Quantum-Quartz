using System;
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

        private bool ended;

        private void Start()
        {
            TimeCounter.Start();
        }

        private void OnEnable()
        {
            InputManager.Restart += OnRestartHandler;
        }

        private void OnDisable()
        {
            InputManager.Restart -= OnRestartHandler;
        }
        
        private void Update()
        {
            TimeCounter.Update(Time.deltaTime);
            timerTxt.text = TimeCounter.Time;
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
