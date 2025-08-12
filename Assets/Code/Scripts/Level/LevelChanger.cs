using System;
using System.Collections;
using System.Linq;
using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Screen;
using Code.Scripts.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.Level
{
    public class LevelChanger : MonoBehaviourSingleton<LevelChanger>
    {
        [SerializeField] private LevelList levelList;
        [SerializeField] private Canvas endLevelCanvas;
        [SerializeField] private Button endLevelFirstSelectedButton;
        [SerializeField] private GameObject playerGO;
        [SerializeField] private InputManager inputManager;

        private int _currentLevel = -1;
        public int CurrentLevel => _currentLevel >= 0 ? _currentLevel : (_currentLevel = levelList.levels.FindIndex(level => level.SceneName == SceneManager.GetActiveScene().name));

        public static event Action LevelEnd;
        public static event Action PlayerTp;

        private void Start()
        {
            if (endLevelCanvas)
            {
                endLevelCanvas.gameObject.SetActive(false);
            }
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

            string nextSceneName = "";
            int currentIndex = CurrentLevel;
            if (currentIndex != -1 && currentIndex + 1 < levelList.levels.Count)
            {
                nextSceneName = levelList.levels[currentIndex + 1].SceneName;
            }
            Stats.FinishLevel(nextSceneName);
            
            EndLevel();

            yield return new WaitForSeconds(time);

            inputManager.EnableUIMap();
            playerGO.SetActive(false);
            endLevelCanvas.gameObject.SetActive(true);
            endLevelFirstSelectedButton.Select();
        }

        public static void EndLevel()
        {
            LevelEnd?.Invoke();
        }

        public void LoadNextLevel()
        {
            int currentIndex = CurrentLevel;
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

        public void LoadLastLevel()
        {
            if (levelList.levels.Count > 0)
            {
                Stats.SetContinueMode(true);
                string sceneName = Stats.GetLastLevelName();
                if (string.IsNullOrWhiteSpace(sceneName) || !levelList.levels.Any((level) => sceneName.Equals(level.SceneName, StringComparison.OrdinalIgnoreCase)))
                {
                    sceneName = levelList.levels[0].SceneName;
                }
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("No hay niveles cargados en el LevelList.");
            }
        }

        public void ReloadLevel()
        {
            Stats.SetContinueMode(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }

        public void LoadLevel(int levelNumber)
        {
            int index = levelNumber - 1;

            if (index >= 0 && index < levelList.levels.Count)
            {
                LoadLevel(levelList.levels[index]);
            }
            else
            {
                Debug.LogError($"Nivel {levelNumber} no existe en el LevelList.");
            }
        }

        public void LoadLevel(LevelList.LevelData level)
        {
            Stats.SetContinueMode(false);
            string sceneName = level.SceneName;
            SceneManager.LoadScene(sceneName);
        }
    }
}