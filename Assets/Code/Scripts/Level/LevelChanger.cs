using System;
using Code.Scripts.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts
{
    public class LevelChanger : MonoBehaviourSingleton<LevelChanger>
    {
        [SerializeField] private string[] levelScenes;
        [SerializeField] private int currentLevel;

        public int CurrentLevel => currentLevel;

        public static event Action LevelEnd;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                LevelEnd?.Invoke();
        }

        public void LoadNextLevel()
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
}