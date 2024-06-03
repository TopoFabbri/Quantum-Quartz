using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private string creditsSceneName;
    [SerializeField] private string mainMenuSceneName;
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PlayGame()
    {
        LoadScene(gameSceneName);
    }
    
    public void ShowCredits()
    {
        LoadScene(creditsSceneName);
    }
    
    public void GoMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
}
