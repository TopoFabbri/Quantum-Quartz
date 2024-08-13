using System;
using Code.Scripts.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Level
{
    public class LevelChanger : MonoBehaviourSingleton<LevelChanger>
    {
        [SerializeField] private string[] levelScenes;
        [SerializeField] private int currentLevel;

        public int CurrentLevel => currentLevel;

        public static event Action LevelEnd;
        public static event Action PlayerTp;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                PlayerTp?.Invoke();
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