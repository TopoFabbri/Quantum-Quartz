using Code.Scripts.Level;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Game
{
    /// <summary>
    /// Class to save and load game stats
    /// </summary>
    public class Stats
    {
        private static Stats _instance;

        private Timer time;
        private Timer level1Time, level2Time, level3Time, level4Time;
        private int deaths;
        private int saveSlot;
        private Timer totalTimer;

        private static Stats Instance => _instance ??= new Stats();

        public static Timer Time => Instance.time;

        /// <summary>
        // Method to select save slot (1, 2, or 3)
        /// <summary>
        public static void SelectSaveSlot(int slot)
        {
            Instance.saveSlot = Mathf.Clamp(slot, 1, 3);
            LoadStats(slot);
        }

        /// <summary>
        // Load player stats from PlayerPrefs
        /// <summary>
        public static void LoadStats(int slot)
        {
            Instance.saveSlot = slot;
            Instance.totalTimer.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_TotalTimer", 0);
            Instance.level1Time.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level1Time", 0);
            Instance.level2Time.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level2Time", 0);
            Instance.level3Time.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level3Time", 0);
            Instance.level4Time.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level4Time", 0);
            Instance.deaths = PlayerPrefs.GetInt($"SaveSlot_{slot}_Deaths", 0);
            
        }

        /// <summary>
        /// Update the provided UI texts with the player's saved stats.
        /// </summary>
        public static void LoadTexts(TextMeshProUGUI totalTimerText,TextMeshProUGUI deathsText)
        {
            totalTimerText.text = Instance.totalTimer.ToStr;
            deathsText.text = PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Deaths", 0).ToString();
        }

        /// <summary>
        // Save player stats to PlayerPrefs
        /// <summary>
        public static void SaveStats()
        {
            int slot = Instance.saveSlot;
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_TotalTimer", Instance.totalTimer.time);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level1Time", Instance.level1Time.time);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level2Time", Instance.level2Time.time);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level3Time", Instance.level3Time.time);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level4Time", Instance.level4Time.time);
            PlayerPrefs.SetInt($"SaveSlot_{slot}_Deaths", Instance.deaths);
            PlayerPrefs.Save();
            
            Timer timerDebug = new()
            {
                time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_TotalTimer", 0)
            };
            
            Debug.Log("Saved, total timer: " + timerDebug.ToStr);
            Debug.Log("Saved, total deaths: " + Instance.deaths);
        }

        /// <summary>
        // Set level time
        /// <summary>
        public static void SetLevelTime(int level, float time)
        {
            switch (level)
            {
                case 1: Instance.level1Time.time = time; break;
                case 2: Instance.level2Time.time = time; break;
                case 3: Instance.level3Time.time = time; break;
                case 4: Instance.level4Time.time = time; break;
            }
            
            Instance.totalTimer.time = Instance.level1Time.time + Instance.level2Time.time + Instance.level3Time.time + Instance.level4Time.time;
        }

        /// <summary>
        // Set deaths
        /// <summary>
        public static void SetDeaths(int deathCount)
        {
            Instance.deaths = deathCount;
        }

        /// <summary>
        // Get level time
        /// <summary>
        public static Timer GetLevelTime(int level)
        {
            return level switch
            {
                1 => Instance.level1Time,
                2 => Instance.level2Time,
                3 => Instance.level3Time,
                4 => Instance.level4Time,
                _ => new Timer()
            };
        }

        /// <summary>
        // Get deaths
        /// <summary>
        public static int GetDeaths()
        {
            return Instance.deaths;
        }
    }
}
