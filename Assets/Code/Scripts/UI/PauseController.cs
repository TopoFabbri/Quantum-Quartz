using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Input;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    
    private bool isPaused = false;
    

    private void OnEnable()
    {
        InputManager.Pause += Pause;
        
        isPaused = false;
    }
    
    private void OnDisable()
    {
        InputManager.Pause -= Pause;
    }

    public void Pause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0;
            pauseCanvas.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseCanvas.SetActive(false);
        }
    }
}
