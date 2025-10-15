using Code.Scripts.Game.Managers;
using Code.Scripts.Game.Triggers;
using Eflatun.SceneReference;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private SceneReference mainMenu;
        [SerializeField] private SceneReference saveFiles;

        [Header("UI Elements")]
        [SerializeField] private GameObject fadeOut;
        [SerializeField] private TextMeshProUGUI versionText;
        
        [Header("Buttons")]
        [SerializeField] private TextMeshProUGUI playButtonText;

        private void Start()
        {
            Stats.DefaultSaveSlot();

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

        public void GoMainMenu()
        {
            LevelChanger.Instance.LoadScene(mainMenu);
        }

        public void GoFileSaves()
        {
            LevelChanger.Instance.LoadScene(saveFiles);
        }

        public void QuitGame()
        {
            Stats.SaveStats();
            Application.Quit();
        }

        public void OnControllerSelection()
        {
            fadeOut.GetComponent<Animation>().Play("Fade-out");
            Invoke(nameof(GoFileSaves), 2);
        }

        public void OnFileSelection(int saveSlot)
        {
            fadeOut.GetComponent<Animation>().Play("Fade-out");
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
