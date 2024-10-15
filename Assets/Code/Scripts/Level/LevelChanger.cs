using System;
using Code.Scripts.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.Level
{
    public class LevelChanger : MonoBehaviourSingleton<LevelChanger>
    {
        [SerializeField] private string[] levelScenes;
        [SerializeField] private int currentLevel;
        [SerializeField] private Canvas endLevelCanvas;
        [SerializeField] private Button endLevelFirstSelectedButton;

        public int CurrentLevel => currentLevel;

        public static event Action LevelEnd;
        public static event Action PlayerTp;

        private void Start()
        {
            //Time.timeScale = 1;
            endLevelCanvas.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                //endLevelCanvas.gameObject.SetActive(true);
                //endLevelFirstSelectedButton.Select();
                //Time.timeScale = 0;
                PlayerTp?.Invoke();
            }
        }
        
        

        public static void EndLevel()
        {
            LevelEnd?.Invoke();
        }

        public void LoadNextLevel()
        {
            if (currentLevel < levelScenes.Length - 1)
            {
                SceneManager.LoadScene(levelScenes[currentLevel + 1]);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}