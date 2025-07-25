using Code.Scripts.Level;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Game
{
    /// <summary>
    /// Class to save and load game stats
    /// </summary>
    public class Stats
    {
        public const int SLOT_COUNT = 3;

        #region Internal Classes
        public abstract class ReadOnlyTotalSlotStats {
            public abstract int Deaths { get; }
            public abstract Timer Timer { get; }
            public abstract string LevelName { get; }
        }

        public abstract class ReadOnlyLevelStats
        {
            public abstract int Deaths { get; }
            public abstract Timer Timer { get; }
            public abstract Vector2 Checkpoint { get; }
            public abstract IReadOnlyCollection<int> Collectibles { get; }
        }

        private class TotalSlotStats : ReadOnlyTotalSlotStats
        {
            private readonly int slot;

            public int deaths;
            public Timer timer;
            public string levelName;

            public override int Deaths => deaths;
            public override Timer Timer => timer;
            public override string LevelName => levelName;

            public TotalSlotStats(int slot)
            {
                this.slot = slot;

                deaths = PlayerPrefs.GetInt($"Slot{slot}_TotalDeaths", 0);
                timer = new Timer(PlayerPrefs.GetFloat($"Slot{slot}_TotalTimer", 0));
                levelName = PlayerPrefs.GetString($"Slot{slot}_LevelName", "");
            }

            public void ResetLevel(ReadOnlyLevelStats levelStats)
            {
                deaths -= levelStats.Deaths;
                timer.time -= levelStats.Timer.time;
            }

            public void Save()
            {
                PlayerPrefs.SetInt($"Slot{slot}_TotalDeaths", deaths);
                PlayerPrefs.SetFloat($"Slot{slot}_TotalTimer", timer.time);
                PlayerPrefs.SetString($"Slot{slot}_LevelName", levelName);
            }
        }

        private class LevelStats : ReadOnlyLevelStats
        {
            private readonly int slot;
            private readonly int level;

            public int deaths;
            public Timer timer;
            public Vector2 checkpoint;
            public HashSet<int> collectibles;

            public override int Deaths => deaths;
            public override Timer Timer => timer;
            public override Vector2 Checkpoint => checkpoint;
            public override IReadOnlyCollection<int> Collectibles => collectibles;

            public LevelStats(int slot, int level)
            {
                this.slot = slot;
                this.level = level;

                timer = new Timer(PlayerPrefs.GetFloat($"Slot{slot}_Level{level}Time", 0));
                deaths = PlayerPrefs.GetInt($"Slot{slot}_Level{level}Deaths", 0);
                checkpoint = new Vector2(
                    PlayerPrefs.GetFloat($"Slot{slot}_Level{level}CheckpointX", float.NegativeInfinity),
                    PlayerPrefs.GetFloat($"Slot{slot}_Level{level}CheckpointY", float.NegativeInfinity)
                );

                //Gets ids from an int32, instead of 32 individual bools
                collectibles = new HashSet<int>();
                int flags = PlayerPrefs.GetInt($"Slot{slot}_Level{level}Collectibles", 0);
                for (int id = 0; id < sizeof(int) * 8; id++)
                {
                    if ((flags & (1 << id)) != 0)
                    {
                        collectibles.Add(id);
                    }
                }
            }

            public void Reset()
            {
                deaths = 0;
                timer.time = 0;
                checkpoint = Vector2.negativeInfinity;
                collectibles.Clear();
            }

            public void Save()
            {
                PlayerPrefs.SetInt($"Slot{slot}_Level{level}Deaths", deaths);
                PlayerPrefs.SetFloat($"Slot{slot}_Level{level}Time", timer.time);
                PlayerPrefs.SetFloat($"Slot{slot}_Level{level}CheckpointX", checkpoint.x);
                PlayerPrefs.SetFloat($"Slot{slot}_Level{level}CheckpointY", checkpoint.y);

                //Makes an int32 out of ids, instead of 32 individual bools
                int flags = 0;
                foreach (int id in collectibles)
                {
                    flags += 1 << id;
                }
                PlayerPrefs.SetInt($"Slot{slot}_Level{level}Collectibles", flags);
            }
        }
        #endregion

        private static Stats _instance;
        private static Stats Instance => _instance ??= new Stats();

        private int saveSlot = -1;
        private bool continueMode = false;
        private TotalSlotStats totalSlotStats = null;
        private LevelStats levelStats = null;

        public static ReadOnlyTotalSlotStats GetTotalSlotStats(int slot)
        {
            if (slot < 1 || slot > SLOT_COUNT)
            {
                return null;
            }

            return new TotalSlotStats(slot);
        }

        public static void SetContinueMode(bool enabled)
        {
            Instance.continueMode = true;
        }

        public static string GetLastLevelName()
        {
            return Instance.totalSlotStats?.levelName;
        }

        public static void ClearSaveSlot(int slot, LevelList levelList)
        {
            PlayerPrefs.DeleteKey($"Slot{slot}_TotalDeaths");
            PlayerPrefs.DeleteKey($"Slot{slot}_TotalTimer");
            PlayerPrefs.DeleteKey($"Slot{slot}_LevelName");
            for (int level = 0; level < levelList.levels.Count; level++)
            {
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}Deaths");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}Time");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}Collectibles");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}CheckpointX");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}CheckpointY");
            }
        }

        // ---------- Save & Load ----------
        public static void SelectSaveSlot(int slot)
        {
            Instance.saveSlot = Mathf.Clamp(slot, 1, SLOT_COUNT);
            Instance.totalSlotStats = new TotalSlotStats(slot);
        }

        public static ReadOnlyLevelStats LoadLevelStats(int level)
        {
            if (Instance.saveSlot == -1)
            {
                SelectSaveSlot(1);
            }

            Instance.levelStats = new LevelStats(Instance.saveSlot, level);

            if (!Instance.continueMode)
            {
                Instance.totalSlotStats.ResetLevel(Instance.levelStats);
                Instance.levelStats.Reset();
            }

            return Instance.levelStats;
        }

        public static void SaveStats()
        {
            Instance.totalSlotStats.Save();
            if (Instance.levelStats != null)
            {
                Instance.totalSlotStats.timer.time += TimeCounter.Time.time - Instance.levelStats.timer.time;
                Instance.levelStats.timer.time = TimeCounter.Time.time;
                Instance.levelStats.Save();
            }

            PlayerPrefs.Save();
        }

        // ---------- Deaths ----------
        public static int GetDeaths()
        {
            return Instance.levelStats.deaths;
        }

        public static void AddDeath()
        {
            Instance.totalSlotStats.deaths++;
            Instance.levelStats.deaths++;
            SaveStats();
        }

        // ---------- Checkpoints ----------
        public static void SaveCheckpoint(Vector2 pos)
        {
            Instance.totalSlotStats.levelName = SceneManager.GetActiveScene().name;
            Instance.levelStats.checkpoint = pos;
            SaveStats();
        }

        public static void FinishLevel(string nextLevelName)
        {
            Instance.totalSlotStats.levelName = nextLevelName;
            Instance.levelStats.checkpoint = Vector2.negativeInfinity;
            SaveStats();
        }

        // ---------- Collectibles ----------
        public static void PickUpCollectible(int id)
        {
            Instance.levelStats.collectibles.Add(id);
            SaveStats();
        }

        public static bool HasCollectible(int id)
        {
            return Instance.levelStats.collectibles.Contains(id);
        }
    }
}
