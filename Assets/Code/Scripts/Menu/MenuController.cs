using Code.Scripts.Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string levelSelectorScene;
        [SerializeField] private string mainMenuSceneName;
        [SerializeField] private string fileSaveSceneName;

        [SerializeField] private LevelList levelList;

        [Header("UI Elements")]
        [SerializeField] private GameObject fadeOut;
        [SerializeField] private OptionsController optionsController;
        [SerializeField] private TextMeshProUGUI versionText;

        

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "SelectControllerMenu")
            {
                FullScreenManager.InitializeFullScreen();
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (versionText != null)
            {
                versionText.text = $"v.{Application.version}";
            }
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void PlayGame()
        {
            if (levelList.levels.Count > 0)
            {
                string sceneName = levelList.levels[0].SceneName;
                LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("No hay niveles cargados en el LevelList.");
            }
        }

        public void GoMainMenu()
        {
            Stats.SaveStats();
            LoadScene(mainMenuSceneName);
        }

        public void GoFileSaves()
        {
            LoadScene(fileSaveSceneName);
        }

        public void GoMainMenuAndTurnOptions()
        {
            LoadScene(mainMenuSceneName);
            optionsController.TurnOptions();
        }

        public void LoadLevel(int levelNumber)
        {
            int index = levelNumber - 1;

            if (index >= 0 && index < levelList.levels.Count)
            {
                string sceneName = levelList.levels[index].SceneName;
                LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"Nivel {levelNumber} no existe en el LevelList.");
            }
        }


        public void QuitGame()
        {
            Stats.SaveStats();
            Application.Quit();
        }

        public void OnControllerSelection()
        {
            fadeOut.GetComponent<UnityEngine.Animation>().Play("Fade-out");
            Invoke(nameof(GoFileSaves), 2);
        }

        public void OnFileSelection(int saveSlot)
        {
            fadeOut.GetComponent<UnityEngine.Animation>().Play("Fade-out");
            Stats.SelectSaveSlot(saveSlot);
            Invoke(nameof(GoMainMenu), 2);
        }
        
        public void OpenLink(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
            else
            {
                Debug.LogWarning("La URL está vacía o no fue asignada.");
            }
        }
    }
}
