using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private string levelSelectorScene;
        [SerializeField] private string mainMenuSceneName;
        [SerializeField] private string level1SceneName;
        [SerializeField] private string level2SceneName;
        [SerializeField] private string level3SceneName;
        [SerializeField] private string level4SceneName;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private GameObject mainMenuButtons;
        [SerializeField] private GameObject fadeOut;
        [SerializeField] private Button optionsFirstButton;
        [SerializeField] private Button creditsBackButton;
        [SerializeField] private Button controlsBackButton;
        [SerializeField] private Button mainMenuButton;


        private void Start()
        {
            optionsPanel.SetActive(false);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(false);

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

        public void TurnCredits()
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (creditsPanel.activeSelf)
            {
                mainMenuButtons.SetActive(false);
                creditsBackButton.Select();
            }
            else
            {
                mainMenuButtons.SetActive(true);
                mainMenuButton.Select();
            }
        }

        public void TurnOptions()
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (optionsPanel.activeSelf)
            {
                mainMenuButtons.SetActive(false);
                optionsFirstButton.Select();
            }
            else
            {
                mainMenuButtons.SetActive(true);
                mainMenuButton.Select();
            }
        }

        public void TurnControls()
        {
            controlsPanel.SetActive(!controlsPanel.activeSelf);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (controlsPanel.activeSelf)
            {
                optionsPanel.SetActive(false);
                controlsBackButton.Select();
            }
            else
            {
                optionsPanel.SetActive(true);
                optionsFirstButton.Select();
            }
        }

        public void GoMainMenu()
        {
            LoadScene(mainMenuSceneName);
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
            Application.Quit();
        }

        public void OnControllerSelection()
        {
            fadeOut.GetComponent<UnityEngine.Animation>().Play("Fade-out");
            Invoke(nameof(GoMainMenu), 2);
            //GoMainMenu();
        }
    }
}