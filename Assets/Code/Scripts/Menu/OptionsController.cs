using System;
using Code.Scripts.Game;
using Code.Scripts.Level;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject levelSelectorPanel;

    [SerializeField] private Button optionsFirstButton;
    [SerializeField] private Button levelSelectorFirstButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button videoButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle timerToggle;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private bool isFullScreen;
    private bool isTimerOn;

    private void Start()
    {
         optionsPanel.SetActive(false);
        // creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        // levelSelectorPanel.SetActive(false);


        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        isTimerOn = PlayerPrefs.GetInt("Timer", 1) == 1;

        //fullScreenToggle.isOn = isFullScreen;
        //timerToggle.isOn = isTimerOn;
        
        if (sfxSlider)
            sfxSlider.value = Settings.SfxVol / 100f;
        
        if (musicSlider)
            musicSlider.value = Settings.MusicVol / 100f;
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

    public void ToggleTimer()
    {
        isTimerOn = timerToggle.isOn;

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
    
    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == controlsButton.gameObject)
            TurnControls();
        else if(EventSystem.current.currentSelectedGameObject == audioButton.gameObject)
            TurnAudio();
        else if(EventSystem.current.currentSelectedGameObject == videoButton.gameObject)
            TurnVideo();
        
    }

    public void OnChangedMusicVolume()
    {
        Settings.MusicVol = musicSlider.value * 100f;
    }
        
    public void OnChangedSFXVolume()
    {
        Settings.SfxVol = sfxSlider.value * 100f;
    }
}