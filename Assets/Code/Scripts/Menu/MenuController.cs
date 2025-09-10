using Code.Scripts.Game;
using Code.Scripts.Level;
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

        [Header("UI Elements")]
        [SerializeField] private GameObject fadeOut;
        [SerializeField] private TextMeshProUGUI versionText;
        
        [Header("Buttons")]
        [SerializeField] private TextMeshProUGUI playButtonText;

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

            //Cambiar texto según el flag de Stats
            if (playButtonText)
            {
                playButtonText.text = Stats.GetLastLevelName() != null ? "CONTINUE" : "PLAY";
            }
        }

        public void PlayGame()
        {
            LevelChanger.Instance.LoadLastLevel();
        }

        private void LoadScene(string sceneName)
        {
            Stats.SaveStats();
            SceneManager.LoadScene(sceneName);
        }

        public void GoMainMenu()
        {
            LoadScene(mainMenuSceneName);
        }

        public void GoFileSaves()
        {
            LoadScene(fileSaveSceneName);
        }

        public void GoMainMenuAndTurnOptions()
        {
            LoadScene(mainMenuSceneName);
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
