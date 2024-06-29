using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string levelSelectorScene;
    [SerializeField] private string creditsSceneName;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string level1SceneName;
    [SerializeField] private string level2SceneName;
    [SerializeField] private string level3SceneName;
    [SerializeField] private string level4SceneName;
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PlayGame()
    {
        LoadScene(levelSelectorScene);
    }
    
    public void ShowCredits()
    {
        LoadScene(creditsSceneName);
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
    
}
