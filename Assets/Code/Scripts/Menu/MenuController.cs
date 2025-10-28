using Code.Scripts.Game.Managers;
using Code.Scripts.Game.Triggers;
using Code.Scripts.Tools;
using Eflatun.SceneReference;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [HeaderPlus("Scene Names")]
        [SerializeField] private SceneReference mainMenu;
        [SerializeField] private SceneReference saveFiles;

        [HeaderPlus("UI Elements")]
        [SerializeField] private GameObject fadeOut;
        [SerializeField] private TextMeshProUGUI versionText;
        
        [HeaderPlus("Buttons")]
        [SerializeField] private TextMeshProUGUI playButtonText;

#if UNITY_EDITOR
        [HeaderPlus("Editor Actions"), InspectorButton("Wipe All Save Data")]
        private void WipeSave()
        {
            PlayerPrefs.DeleteAll();
        }
#endif

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
