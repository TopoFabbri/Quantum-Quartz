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
        private int level1Time, level2Time, level3Time, level4Time;
        private int deaths;
        private int saveSlot;
        private string totalTimer;

        private static Stats Instance => _instance ??= new Stats();

        public static Timer Time => Instance.time;

        /// <summary>
        /// Set last run's time
        /// </summary>
        /// <param name="time">Value to set</param>
        public static void SetTime(Timer time)
        {
            Instance.time = time;
            Instance.totalTimer = time.ToStr;
        }

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
            Instance.level1Time = PlayerPrefs.GetInt($"SaveSlot_{slot}_Level1Time", 0);
            Instance.level2Time = PlayerPrefs.GetInt($"SaveSlot_{slot}_Level2Time", 0);
            Instance.level3Time = PlayerPrefs.GetInt($"SaveSlot_{slot}_Level3Time", 0);
            Instance.level4Time = PlayerPrefs.GetInt($"SaveSlot_{slot}_Level4Time", 0);
            Instance.deaths = PlayerPrefs.GetInt($"SaveSlot_{slot}_Deaths", 0);
            
        }

        /// <summary>
        /// Update the provided UI texts with the player's saved stats.
        /// </summary>
        public static void LoadTexts(TextMeshProUGUI totalTimerText, TextMeshProUGUI level1TimerText,TextMeshProUGUI level2TimerText, TextMeshProUGUI level3TimerText, TextMeshProUGUI level4TimerText, TextMeshProUGUI deathsText)
        {
            int totalTime = Instance.level1Time + Instance.level2Time + Instance.level3Time + Instance.level4Time;
            //Instance.totalTimer = totalTime.ToString();
            
            level1TimerText.text = PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Level1Time", 0).ToString();
            level2TimerText.text = PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Level2Time", 0).ToString();
            level3TimerText.text = PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Level3Time", 0).ToString();
            level4TimerText.text = PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Level4Time", 0).ToString();
            
            if(PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Level1Time", 0) == 0)
                level1TimerText.text = "00:00:00";
            if(PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Level2Time", 0) == 0)
                level2TimerText.text = "00:00:00";
            if(PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Level3Time", 0) == 0)
                level3TimerText.text = "00:00:00";
            if(PlayerPrefs.GetInt($"SaveSlot_{Instance.saveSlot}_Level4Time", 0) == 0)
                level4TimerText.text = "00:00:00";
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
            PlayerPrefs.SetInt($"SaveSlot_{slot}_Level1Time", Instance.level1Time);
            PlayerPrefs.SetInt($"SaveSlot_{slot}_Level2Time", Instance.level2Time);
            PlayerPrefs.SetInt($"SaveSlot_{slot}_Level3Time", Instance.level3Time);
            PlayerPrefs.SetInt($"SaveSlot_{slot}_Level4Time", Instance.level4Time);
            PlayerPrefs.SetInt($"SaveSlot_{slot}_Deaths", Instance.deaths);
            PlayerPrefs.Save();
            Debug.Log("Saved, total timer: " + PlayerPrefs.GetString($"SaveSlot_{slot}_TotalTimer", "00:00:00"));
        }

        /// <summary>
        // Set level time
        /// <summary>
        public static void SetLevelTime(int level, int time)
        {
            switch (level)
            {
                case 1: Instance.level1Time = time; break;
                case 2: Instance.level2Time = time; break;
                case 3: Instance.level3Time = time; break;
                case 4: Instance.level4Time = time; break;
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
        public static int GetLevelTime(int level)
        {
            return level switch
            {
                1 => Instance.level1Time,
                2 => Instance.level2Time,
                3 => Instance.level3Time,
                4 => Instance.level4Time,
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
