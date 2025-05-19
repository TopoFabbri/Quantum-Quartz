using System;
using AYellowpaper.SerializedCollections;
using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Level;
using Code.Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [System.Serializable]
    public struct MappingImages
    {
        public Sprite keyboard;
        public Sprite playstation;
        public Sprite xbox;
    }

    [HeaderPlus("Main")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button optionsFirstButton;
    [SerializeField] private Button mainMenuButton;

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
        if (timerToggle)
        {
            bool isTimerOn = PlayerPrefs.GetInt("Timer", 1) == 1;
            timerToggle.isOn = isTimerOn;
        }

        if (sfxSlider)
            sfxSlider.value = Settings.SfxVol / 100f;

        if (musicSlider)
            musicSlider.value = Settings.MusicVol / 100f;

        if (controlsDropdown)
        {
            string controlsMapping = PlayerPrefs.GetString("ControlsMapping", null);
            int index = controlsDropdown.options.FindIndex((item) => item.text.Equals(controlsMapping));
            controlsDropdown.value = Math.Min(0, index);
            SetControlsMapping();
        }
    }

    public void SetMusicVolume()
    {
        Settings.MusicVol = musicSlider.value * 100f;
    }

    public void SetSfxVolume()
    {
        Settings.SfxVol = sfxSlider.value * 100f;
    }

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
        foreach (InputDevice device in InputManager.Input.devices)
        {
            if (device is UnityEngine.InputSystem.DualShock.DualShockGamepad)
            {
                controlsImage.sprite = controlMappingImages[controlsMapping].playstation;
                break;
            }
            else if (device is UnityEngine.InputSystem.XInput.XInputController)
            {
                controlsImage.sprite = controlMappingImages[controlsMapping].xbox;
                break;
            }
            else
            {
                controlsImage.sprite = controlMappingImages[controlsMapping].keyboard;
                break;
            }
        }
    }

    public void ToggleTimer()
    {
        bool isTimerOn = timerToggle.isOn;

        GameManager.Instance.isTimerOn = isTimerOn;
        PlayerPrefs.SetInt("Timer", isTimerOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void TurnCredits()
    {
        creditsPanel.SetActive(!creditsPanel.activeSelf);

        if (creditsPanel.activeSelf)
        {
            creditsButton.Select();
        }
        else
        {
            mainMenuButton.Select();
        }
    }

    public void TurnOptions()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);

        if (optionsPanel.activeSelf)
        {
            optionsFirstButton.Select();
        }
        else
        {
            mainMenuButton.Select();
        }
    }

    public void TurnLevelSelector()
    {
        levelSelectorPanel.SetActive(!levelSelectorPanel.activeSelf);

        if (levelSelectorPanel.activeSelf)
        {
            levelSelectorFirstButton.Select();
        }
        else
        {
            mainMenuButton.Select();
        }
    }

    public void TurnControls()
    {
        controlsPanel.SetActive(true);

        if (controlsPanel.activeSelf)
        {
            videoPanel.SetActive(false);
            audioPanel.SetActive(false);

            controlsButton.Select();
        }
        else
        {
            optionsFirstButton.Select();
        }
    }

    public void TurnVideo()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        videoPanel.SetActive(true);

        if (videoPanel.activeSelf)
        {
            controlsPanel.SetActive(false);
            audioPanel.SetActive(false);

            videoButton.Select();
        }
        else
        {
            optionsFirstButton.Select();
        }
    }

    public void TurnAudio()
    {
        audioPanel.SetActive(true);

        if (audioPanel.activeSelf)
        {
            controlsPanel.SetActive(false);
            videoPanel.SetActive(false);

            audioButton.Select();
        }
        else
        {
            optionsFirstButton.Select();
        }
    }

    private void OnUIBack()
    {
        optionsPanel?.SetActive(false);
        levelSelectorPanel?.SetActive(false);
        creditsPanel?.SetActive(false);
        
        mainMenuButton.Select();
    }
}