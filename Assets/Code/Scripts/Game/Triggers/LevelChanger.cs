using Code.Scripts.Game.Managers;
using Code.Scripts.Input;
using Code.Scripts.Menu;
using Code.Scripts.Tools;
using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.Game.Triggers
{
    public class LevelChanger : MonoBehaviourSingleton<LevelChanger>
    {
        [SerializeField] private LevelList levelList;
        [SerializeField] private Canvas endLevelCanvas;
        [SerializeField] private Button endLevelFirstSelectedButton;
        [SerializeField] private GameObject playerGO;
        [SerializeField] private SceneReference menuScene;

        [Header("Level Options")]
        [Tooltip("Si es verdadero, este nivel es un Gauntlet y volverá al menú al terminar.")]
        [SerializeField] private bool isGauntletLevel = false;

        private int _currentLevel = -1;
        public int CurrentLevel => _currentLevel >= 0
            ? _currentLevel
            : (_currentLevel = levelList.levels.FindIndex(level =>
                level.SceneName == SceneManager.GetActiveScene().name));

        public LevelList LevelList => levelList;

        public static event Action LevelEnd;
        public static event Action PlayerTp;

        private void Start()
        {
            if (endLevelCanvas)
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

            string nextSceneName = "";
            int currentIndex = CurrentLevel;

            if (!isGauntletLevel && currentIndex != -1 && currentIndex + 1 < levelList.levels.Count)
            {
                nextSceneName = levelList.levels[currentIndex + 1].SceneName;
            }

            Stats.FinishLevel(nextSceneName);
            yield return new WaitForSeconds(time);

            InputManager.Instance.EnableUIMap();
            playerGO.SetActive(false);
            endLevelCanvas.gameObject.SetActive(true);
            endLevelFirstSelectedButton.Select();
        }

        public void EndLevel()
        {
            endLevelCanvas.gameObject.SetActive(false);
            LevelEnd?.Invoke();
        }

        public void LoadNextLevel()
        {
            // Si el nivel es un Gauntlet, saltamos directamente al menú.
            if (isGauntletLevel)
            {
                Debug.Log("Nivel Gauntlet completado. Volviendo al menú principal.");
                LoadMenu();
                return;
            }

            int currentIndex = CurrentLevel;
            if (currentIndex != -1 && currentIndex + 1 < levelList.levels.Count)
            {
                string nextSceneName = levelList.levels[currentIndex + 1].SceneName;
                LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("No hay más niveles, volviendo al menú principal.");
                LoadMenu();
            }
        }

        public void LoadLastLevel()
        {
            if (levelList.levels.Count > 0)
            {
                Stats.SetContinueMode(true);
                string sceneName = Stats.GetLastLevelName();

                if (string.IsNullOrWhiteSpace(sceneName) ||
                    !levelList.levels.Any(level => sceneName.Equals(level.SceneName, StringComparison.OrdinalIgnoreCase)))
                {
                    sceneName = levelList.levels[0].SceneName;
                }

                LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("No hay niveles cargados en el LevelList.");
            }
        }

        public void ReloadLevel()
        {
            Stats.SetContinueMode(false);
            LoadScene(SceneManager.GetActiveScene().name);
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

        public void LoadLevel(LevelList.LevelData level, bool continueMode = false)
        {
            Stats.SetContinueMode(continueMode);
            string sceneName = level.SceneName;
            LoadScene(sceneName);
        }

        public void LoadScene(SceneReference scene) => LoadScene(scene.Name);

        public void LoadScene(string sceneName)
        {
            Stats.SaveStats();
            SceneManager.LoadScene(sceneName);
        }

        private void LoadMenu()
        {
            if (menuScene != null && !string.IsNullOrEmpty(menuScene.Name))
            {
                LoadScene(menuScene.Name);
            }
            else
            {
                Debug.LogError("No se asignó la escena del menú en el LevelChanger.");
            }
        }
    }
}
