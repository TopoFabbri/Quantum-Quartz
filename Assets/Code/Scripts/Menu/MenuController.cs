using Code.Scripts.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private string levelSelectorScene;
        [SerializeField] private string mainMenuSceneName;
        [SerializeField] private string fileSaveSceneName;
        [SerializeField] private string level1SceneName;
        [SerializeField] private string level2SceneName;
        [SerializeField] private string level3SceneName;
        [SerializeField] private string level4SceneName;

        [SerializeField] private GameObject fadeOut;
        [SerializeField] private OptionsController optionsController;

        private bool isFullScreen;

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

        public void LoadLevel1()
        {
            LoadScene(level1SceneName);
        }

        public void LoadLevel2()
        {
            LoadScene(level2SceneName);
        }

        public void LoadLevel3()
        {
            LoadScene(level3SceneName);
        }

        public void LoadLevel4()
        {
            LoadScene(level4SceneName);
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
            //GoMainMenu();
        }

        public void OnFileSelection(int saveSlot)
        {
            fadeOut.GetComponent<UnityEngine.Animation>().Play("Fade-out");
            Stats.SelectSaveSlot(saveSlot);
            Invoke(nameof(GoMainMenu), 2);
        }
    }
}