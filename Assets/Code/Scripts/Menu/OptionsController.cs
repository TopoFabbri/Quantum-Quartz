using AYellowpaper.SerializedCollections;
using Code.Scripts.Game.Managers;
using Code.Scripts.Input;
using Code.Scripts.Tools;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private InputActionReference cancelAction;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private Button optionsFirstButton;
        [SerializeField] private Button mainMenuButton;

        [HeaderPlus("Controls")]
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private Button controlsButton;
        [SerializeField] private TMP_Dropdown controlsDropdown;
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
        
        [HeaderPlus("Gauntlet Selector")]
        [SerializeField] private GameObject gauntletSelectorPanel;
        [SerializeField] private Button gauntletSelectorFirstButton;

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
            cancelAction.action.performed += OnCancel;
        }

        private void OnDisable()
        {
            cancelAction.action.performed -= OnCancel;
        }

        private void OnDestroy()
        {
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
            sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
        }

        private static readonly Type[] nonCancelConsumers = { typeof(TMP_Dropdown) };
        void OnCancel(InputAction.CallbackContext context)
        {
            GameObject selected = EventSystem.current?.currentSelectedGameObject;
            BaseEventData data = new BaseEventData(EventSystem.current);
            if (selected != null)
            {
                bool consumed = false;
                ExecuteEvents.Execute<ICancelHandler>(selected, data, (h, e) => {
                    h.OnCancel(e);
                    if (!nonCancelConsumers.Contains(h.GetType()))
                    {
                        consumed = true;
                    }
                });

                if (consumed && !data.used)
                {
                    data.Use();
                }
            }

            if (!data.used)
            {
                ExecuteEvents.Execute<ICancelHandler>(gameObject, data, (h, e) => {
                    h.OnCancel(e);
                });
            }
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
            if (InputManager.HasInstance)
            {
                InputManager.Instance.SwitchGameMap(controlsMapping);
            }

            if (controlMappingImages.TryGetValue(controlsMapping, out var mappingImages))
            {
                controlsImage.color = Color.white;
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
                controlsImage.color = Color.clear;
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
        }
        
        public void TurnGauntletsSelector()
        {
            gauntletSelectorPanel.SetActive(!gauntletSelectorPanel.activeSelf);
            (gauntletSelectorPanel.activeSelf ? gauntletSelectorFirstButton : mainMenuButton).Select();
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

        public void OnUIBack()
        {
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                TurnOptions();
                return;
            }

            if (levelSelectorPanel != null && levelSelectorPanel.activeSelf)
            {
                TurnLevelSelector();
                return;
            }

            if (creditsPanel != null && creditsPanel.activeSelf)
            {
                TurnCredits();
                return;
            }

            if (gauntletSelectorPanel != null && gauntletSelectorPanel.activeSelf)
            {
                TurnGauntletsSelector();
                return;
            }
        }

        public void ResetSaveData()
        {
            Stats.ClearSaveSlot(1, null);
            Stats.ClearSaveSlot(2, null);
            Stats.ClearSaveSlot(3, null);
        }
    }
}