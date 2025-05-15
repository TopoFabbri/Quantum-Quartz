using System;
using System.Collections;
using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Tools;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.Level
{
    public class LevelChanger : MonoBehaviourSingleton<LevelChanger>
    {
        [SerializeField] private SceneReference nextScene;
        [SerializeField] private int currentLevel;
        [SerializeField] private Canvas endLevelCanvas;
        [SerializeField] private Button endLevelFirstSelectedButton;
        [SerializeField] private GameObject playerGO;
        [SerializeField] private InputManager inputManager;

        public int CurrentLevel => currentLevel;

        public static event Action LevelEnd;
        public static event Action PlayerTp;

        private void Start()
        {
            endLevelCanvas.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerTp?.Invoke();

                StartCoroutine(ShowEndLevelScreen(2));
            }
        }

        private IEnumerator ShowEndLevelScreen(float time)
        {
            TimeCounter.Stop();
            Stats.SetLevelTime(currentLevel + 1, TimeCounter.Time.time);
            yield return new WaitForSeconds(time);
            playerGO.SetActive(false);
            endLevelCanvas.gameObject.SetActive(true);
            endLevelFirstSelectedButton.Select();
            inputManager.EnableUIMap();
        }

        public static void EndLevel()
        {
            LevelEnd?.Invoke();
        }

        public void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }

        public void LoadNextLevel()
        {
            if (nextScene.State == SceneReferenceState.Regular)
            {
                SceneManager.LoadScene(nextScene.Path);
            }
        }
    }
}