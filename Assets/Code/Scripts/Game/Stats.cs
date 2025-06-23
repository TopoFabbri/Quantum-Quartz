using Code.Scripts.Level;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Game
{
    /// <summary>
    /// Class to save and load game stats
    /// </summary>
    public class Stats
    {
        public static int Deaths => Instance.deaths;

        private static Stats _instance;
        private static Stats Instance => _instance ??= new Stats();

        private Dictionary<int, Timer> levelTimes = new Dictionary<int, Timer>();
        private int deaths;
        private int saveSlot;
        private Timer totalTimer;
        private Dictionary<int, HashSet<int>> collectibles = new Dictionary<int, HashSet<int>>();

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
            
            for (int i = 1; PlayerPrefs.HasKey($"SaveSlot_{slot}_Level{i}Time"); i++)
            {
                Timer newTimer = new Timer(PlayerPrefs.GetFloat($"SaveSlot_{slot}_Level{i}Time", 0));
                if (!Instance.levelTimes.TryAdd(i - 1, newTimer))
                {
                    Instance.levelTimes[i - 1] = newTimer;
                }
            }
            Instance.deaths = PlayerPrefs.GetInt($"SaveSlot_{slot}_Deaths", 0);

            for (int i = 1; PlayerPrefs.HasKey($"SaveSlot_{slot}_Level{i}Collectibles"); i++)
            {
                //Gets ids from an int32, instead of 32 individual bools
                Instance.collectibles.TryAdd(i - 1, new HashSet<int>());
                int flags = PlayerPrefs.GetInt($"SaveSlot_{slot}_Level{i}Collectibles", 0);
                for (int id = 0; id < sizeof(int) * 8; id++)
                {
                    if ((flags & (1 << id)) != 0)
                    {
                        Instance.collectibles[i - 1].Add(id);
                    }
                }
            }
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
            foreach (int key in Instance.levelTimes.Keys)
            {
                PlayerPrefs.SetFloat($"SaveSlot_{slot}_Level{key}Time", Instance.levelTimes[key].time);
            }
            PlayerPrefs.SetInt($"SaveSlot_{slot}_Deaths", Instance.deaths);

            foreach (int key in Instance.collectibles.Keys)
            {
                //Makes an int32 out of ids, instead of 32 individual bools
                int flags = 0;
                foreach (int id in Instance.collectibles[key])
                {
                    flags += 1 << id;
                }
                PlayerPrefs.SetInt($"SaveSlot_{slot}_Level{key}Collectibles", flags);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        // Set level time
        /// <summary>
        public static void SetLevelTime(int level, float time)
        {
            if (!Instance.levelTimes.TryAdd(level - 1, new Timer(time))) {
                Instance.levelTimes[level - 1] = new Timer(time);
            }
            Instance.totalTimer.time = 0;
            foreach (int key in Instance.levelTimes.Keys)
            {
                Instance.totalTimer.time += Instance.levelTimes[key].time;
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
        public static Timer GetLevelTime(int level)
        {
            return Instance.levelTimes.GetValueOrDefault(level - 1, new Timer());
        }

        public static void PickUpCollectible(int level, int id)
        {
            Instance.collectibles.TryAdd(level, new HashSet<int>());
            Instance.collectibles[level].Add(id);
        }

        public static bool HasCollectible(int level, int id)
        {
            return Instance.collectibles.GetValueOrDefault(level, new HashSet<int>()).Contains(id);
        }
    }
}
