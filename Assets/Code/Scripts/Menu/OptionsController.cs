using System;
using AYellowpaper.SerializedCollections;
using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.Scripts.Menu
{
    public class OptionsController : MonoBehaviour
    {
        [Serializable]
        public struct MappingImages
        {
            public Sprite keyboard;
            public Sprite playstation;
            public Sprite xbox;
        }

        public static OptionsController Instance { get; private set; }

        [HeaderPlus("Main")]
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private Button optionsFirstButton;
        [SerializeField] private Button mainMenuButton;

        [HeaderPlus("In Game")]
        [SerializeField] private GameObject gameHUD;

        [HeaderPlus("Controls")]
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private Button controlsButton;
        [SerializeField] private TMP_Dropdown controlsDropdown;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Image controlsImage;
        [SerializeField] private SerializedDictionary<string, MappingImages> controlMappingImages;

        [HeaderPlus("Video")]
        [SerializeField] private GameObject videoPanel;
        [SerializeField] private Button videoButton;
        [SerializeField] private Toggle fullScreenToggle;
        [SerializeField] private Toggle timerToggle;

        [HeaderPlus("Audio")]
        [SerializeField] private GameObject audioPanel;
        [SerializeField] private Button audioButton;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [HeaderPlus("Level Selector")]
        [SerializeField] private GameObject levelSelectorPanel;
        [SerializeField] private Button levelSelectorFirstButton;

        [HeaderPlus("Credits")]
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private Button creditsButton;

        private void Start()
        {
            InitializeTimerToggle();
            InitializeAudioSliders();
            InitializeControlsDropdown();
        }

        private void OnEnable()
        {
            gameHUD?.SetActive(false);
        }

        private void OnDisable()
        {
            gameHUD?.SetActive(true);
        }

        private void OnDestroy()
        {
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
            sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
        }

        private void InitializeTimerToggle()
        {
            if (!timerToggle) return;

            bool isTimerOn = Settings.ShowGameTimer;
            timerToggle.isOn = isTimerOn;
        }

        private void InitializeAudioSliders()
        {
            if (musicSlider)
            {
                musicSlider.value = Settings.MusicVol / 100f;
                musicSlider.onValueChanged.AddListener(SetMusicVolume);
            }

            if (sfxSlider)
            {
                sfxSlider.value = Settings.SfxVol / 100f;
                sfxSlider.onValueChanged.AddListener(SetSfxVolume);
            }
        }

        private void InitializeControlsDropdown()
        {
            if (!controlsDropdown) return;

            string controlsMapping = PlayerPrefs.GetString("ControlsMapping", null);
            int index = controlsDropdown.options.FindIndex(item => item.text.Equals(controlsMapping));
            controlsDropdown.value = index >= 0 ? index : 0;
            SetControlsMapping();
        }

        private static void SetMusicVolume(float value) => Settings.MusicVol = value * 100f;
        private static void SetSfxVolume(float value) => Settings.SfxVol = value * 100f;

        public void SetFullScreen()
        {
            bool isFullScreen = fullScreenToggle.isOn;
            FullScreenManager.SetFullScreen(isFullScreen);
        }

        public void SetControlsMapping()
        {
            string controlsMapping = controlsDropdown.options[controlsDropdown.value].text;
            PlayerPrefs.SetString("ControlsMapping", controlsMapping);
            inputManager?.SwitchGameMap(controlsMapping);

            if (controlMappingImages.TryGetValue(controlsMapping, out var mappingImages))
            {
                switch (InputManager.GetControlScheme())
                {
                    case InputManager.ControlScheme.Gamepad:
                        controlsImage.sprite = mappingImages.xbox;
                        break;
                    case InputManager.ControlScheme.DS4:
                        controlsImage.sprite = mappingImages.playstation;
                        break;
                    default:
                        controlsImage.sprite = mappingImages.keyboard;
                        break;
                }
            }
            else
            {
                controlsImage.sprite = null;
            }
        }

        //Método que se llama desde el Toggle en UI
        public void ToggleTimer()
        {
            bool isTimerOn = timerToggle.isOn;
            Settings.ShowGameTimer = isTimerOn;
        }

        //Métodos para abrir/cerrar paneles
        public void TurnCredits()
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);
            (creditsPanel.activeSelf ? creditsButton : mainMenuButton).Select();
        }

        public void TurnOptions()
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
            (optionsPanel.activeSelf ? optionsFirstButton : mainMenuButton).Select();
        }

        public void TurnLevelSelector()
        {
            levelSelectorPanel.SetActive(!levelSelectorPanel.activeSelf);
            (levelSelectorPanel.activeSelf ? levelSelectorFirstButton : mainMenuButton).Select();
            if(levelSelectorPanel.activeSelf) levelSelectorFirstButton.Select();
            
        }

        public void TurnControls()
        {
            controlsPanel.SetActive(true);
            videoPanel.SetActive(false);
            audioPanel.SetActive(false);
            controlsButton.Select();
        }

        public void TurnVideo()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            videoPanel.SetActive(true);
            controlsPanel.SetActive(false);
            audioPanel.SetActive(false);
            videoButton.Select();
        }

        public void TurnAudio()
        {
            audioPanel.SetActive(true);
            controlsPanel.SetActive(false);
            videoPanel.SetActive(false);
            audioButton.Select();
        }

        private void OnUIBack()
        {
            optionsPanel?.SetActive(false);
            levelSelectorPanel?.SetActive(false);
            creditsPanel?.SetActive(false);
            mainMenuButton.Select();
        }
        
        public void ResetSaveData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            Debug.Log("PlayerPrefs reseteadas.");
        }

    }
}
