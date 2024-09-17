using System;
using System.Collections;
using System.Collections.Generic;
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

    private bool isFullScreen = true;
    private void Start()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        
        isFullScreen = PlayerPrefs.GetInt("FullScreen", 0) == 1;
        Screen.fullScreen = isFullScreen;
    }

    public void SetFullScreen()
    {
        isFullScreen = !Screen.fullScreen;
        Screen.fullScreen = isFullScreen;
        
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
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
}
