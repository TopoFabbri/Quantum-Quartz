using Code.Scripts.Game;
using Code.Scripts.Level;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject mainMenuButtons;

    [SerializeField] private Button optionsFirstButton;
    [SerializeField] private Button creditsBackButton;
    [SerializeField] private Button controlsBackButton;
    [SerializeField] private Button videoFirstButton;
    [SerializeField] private Button audioFirstButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle timerToggle;
    [SerializeField] private Scrollbar musicSlider;
    [SerializeField] private Scrollbar sfxSlider;

    private bool isFullScreen;
    private bool isTimerOn;

    private void Start()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);


        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        isTimerOn = PlayerPrefs.GetInt("Timer", 1) == 1;

        fullScreenToggle.isOn = isFullScreen;
        timerToggle.isOn = isTimerOn;
        
        if (sfxSlider)
            sfxSlider.value = Settings.SfxVol / 100f;
        
        if (musicSlider)
            musicSlider.value = Settings.MusicVol / 100f;
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

    public void TurnVideo()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        videoPanel.SetActive(!videoPanel.activeSelf);

        if (videoPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
            videoFirstButton.Select();
        }
        else
        {
            optionsPanel.SetActive(true);
            optionsFirstButton.Select();
        }
    }

    public void TurnAudio()
    {
        audioPanel.SetActive(!audioPanel.activeSelf);

        if (audioPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
            audioFirstButton.Select();
        }
        else
        {
            optionsPanel.SetActive(true);
            optionsFirstButton.Select();
        }
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