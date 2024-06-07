using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private string[] levelScenes; 

    [SerializeField] private int currentLevel = 0;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        if (currentLevel < levelScenes.Length)
        {
            SceneManager.LoadScene(levelScenes[currentLevel+1]);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }
}