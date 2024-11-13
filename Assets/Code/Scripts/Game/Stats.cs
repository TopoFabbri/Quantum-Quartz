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
        private string totalTimer;

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
            Instance.totalTimer = PlayerPrefs.GetString($"SaveSlot_{slot}_TotalTimer", "00:00:00");
            Instance.level1Time.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level1Time", 0);
            Instance.level2Time.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level2Time", 0);
            Instance.level3Time.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level3Time", 0);
            Instance.level4Time.time = PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level4Time", 0);
            Instance.deaths = PlayerPrefs.GetInt($"SaveSlot_{slot}_Deaths", 0);
            
        }

        /// <summary>
        /// Update the provided UI texts with the player's saved stats.
        /// </summary>
        public static void LoadTexts(TextMeshProUGUI totalTimerText, TextMeshProUGUI level1TimerText,TextMeshProUGUI level2TimerText, TextMeshProUGUI level3TimerText, TextMeshProUGUI level4TimerText, TextMeshProUGUI deathsText)
        {
            float totalTime = Instance.level1Time.time + Instance.level2Time.time + Instance.level3Time.time + Instance.level4Time.time;
            Instance.totalTimer = totalTime.ToString();

            Instance.level1Time.time = PlayerPrefs.GetFloat($"SaveSlot_{Instance.saveSlot}_Level1Time", 0);
            Instance.level2Time.time = PlayerPrefs.GetFloat($"SaveSlot_{Instance.saveSlot}_Level2Time", 0);
            Instance.level3Time.time = PlayerPrefs.GetFloat($"SaveSlot_{Instance.saveSlot}_Level3Time", 0);
            Instance.level4Time.time = PlayerPrefs.GetFloat($"SaveSlot_{Instance.saveSlot}_Level4Time", 0);
            
            level1TimerText.text = Instance.level1Time.ToStr;
            level2TimerText.text = Instance.level2Time.ToStr;
            level3TimerText.text = Instance.level3Time.ToStr;
            level4TimerText.text = Instance.level4Time.ToStr;
            
            if(PlayerPrefs.GetString($"SaveSlot_{Instance.saveSlot}_TotalTimer", "00:00:00") == "0")
                Instance.totalTimer = "00:00:00";
            
            totalTimerText.text = Instance.totalTimer;
            deathsText.text = PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Deaths", 0).ToString();
        }

        /// <summary>
        // Save player stats to PlayerPrefs
        /// <summary>
        public static void SaveStats()
        {
            int slot = Instance.saveSlot;
            PlayerPrefs.SetString($"SaveSlot_{slot}_TotalTimer", Instance.totalTimer);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level1Time", Instance.level1Time.time);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level2Time", Instance.level2Time.time);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level3Time", Instance.level3Time.time);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level4Time", Instance.level4Time.time);
            PlayerPrefs.SetFloat($"SaveSlot_{slot}_Deaths", Instance.deaths);
            PlayerPrefs.Save();
            Debug.Log("Saved, total timer: " + PlayerPrefs.GetString($"SaveSlot_{slot}_TotalTimer", "00:00:00"));
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
        public static float GetLevelTime(int level)
        {
            return level switch
            {
                1 => Instance.level1Time.time,
                2 => Instance.level2Time.time,
                3 => Instance.level3Time.time,
                4 => Instance.level4Time.time,
                _ => 0
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
