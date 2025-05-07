using Code.Scripts.Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private string levelSelectorScene;
        [SerializeField] private string mainMenuSceneName;
        [SerializeField] private string fileSaveSceneName;

        [SerializeField] private List<string> levelSceneNames = new List<string>();

        [SerializeField] private GameObject fadeOut;
        [SerializeField] private OptionsController optionsController;

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "SelectControllerMenu")
            {
                FullScreenManager.InitializeFullScreen();
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void PlayGame()
        {
            LoadScene(levelSelectorScene);
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

            if (index >= 0 && index < levelSceneNames.Count)
            {
                LoadScene(levelSceneNames[index]);
            }
            else
            {
                Debug.LogError($"Nivel {levelNumber} no existe en la lista de niveles.");
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
    }
}
