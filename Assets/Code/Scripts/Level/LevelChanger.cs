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
        [SerializeField] private LevelList levelList;
        [SerializeField] private int currentLevel;
        [SerializeField] private Canvas endLevelCanvas;
        [SerializeField] private Button endLevelFirstSelectedButton;
        [SerializeField] private GameObject playerGO;
        [SerializeField] private InputManager inputManager;

        public int CurrentLevel => currentLevel + 1;

        public static event Action LevelEnd;
        public static event Action PlayerTp;

        private void Start()
        {
            endLevelCanvas.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.isTrigger && other.CompareTag("Player"))
            {
                PlayerTp?.Invoke();

                StartCoroutine(ShowEndLevelScreen(2));
            }
        }

        private IEnumerator ShowEndLevelScreen(float time)
        {
            TimeCounter.Stop();
            Stats.SetLevelTime(CurrentLevel, TimeCounter.Time.time);
            yield return new WaitForSeconds(time);
            Stats.SaveStats();
            inputManager.EnableUIMap();
            playerGO.SetActive(false);
            endLevelCanvas.gameObject.SetActive(true);
            endLevelFirstSelectedButton.Select();
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
            string currentSceneName = SceneManager.GetActiveScene().name;

            int currentIndex = levelList.levels.FindIndex(level => level.SceneName == currentSceneName);

            if (currentIndex != -1 && currentIndex + 1 < levelList.levels.Count)
            {
                string nextSceneName = levelList.levels[currentIndex + 1].SceneName;
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("No hay siguiente nivel o el nivel actual no está en la lista.");
            }
        }
    }
}