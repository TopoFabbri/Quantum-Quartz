using Code.Scripts.Level;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Code.Scripts.Colors;

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
            public abstract int TotalDeaths { get; }
            public abstract Timer TotalTimer { get; }
            public abstract string CurLevelName { get; }
        }

        public abstract class ReadOnlyLevelStats
        {
            public abstract int TotalDeaths { get; }
            public abstract Timer TotalTimer { get; }
            public abstract Timer RecordTimer { get; }
            public abstract IReadOnlyCollection<int> Collectibles { get; }
            public abstract int CurDeaths { get; }
            public abstract Timer CurTimer { get; }
            public abstract Vector2 CurCheckpoint { get; }
            public abstract IReadOnlyCollection<ColorSwitcher.QColor> CurColors { get; }
        }

        private class TotalSlotStats : ReadOnlyTotalSlotStats
        {
            private readonly int slot;

            public int totalDeaths;
            public Timer totalTimer;
            public string curLevelName;

            public override int TotalDeaths => totalDeaths;
            public override Timer TotalTimer => totalTimer;
            public override string CurLevelName => curLevelName;

            public TotalSlotStats(int slot)
            {
                this.slot = slot;

                totalDeaths = PlayerPrefs.GetInt($"Slot{slot}_TotalDeaths", 0);
                totalTimer = new Timer(PlayerPrefs.GetFloat($"Slot{slot}_TotalTimer", 0));
                curLevelName = PlayerPrefs.GetString($"Slot{slot}_LevelName", "");
            }

            public void Save()
            {
                PlayerPrefs.SetInt($"Slot{slot}_TotalDeaths", totalDeaths);
                PlayerPrefs.SetFloat($"Slot{slot}_TotalTimer", totalTimer.time);
                PlayerPrefs.SetString($"Slot{slot}_LevelName", curLevelName);
            }

            public static void Clear(int slot)
            {
                PlayerPrefs.DeleteKey($"Slot{slot}_TotalDeaths");
                PlayerPrefs.DeleteKey($"Slot{slot}_TotalTimer");
                PlayerPrefs.DeleteKey($"Slot{slot}_LevelName");
            }
        }

        private class LevelStats : ReadOnlyLevelStats
        {
            private readonly int slot;
            private readonly int level;

            public int totalDeaths;
            public Timer totalTimer;
            public Timer recordTimer;
            public HashSet<int> collectibles;
            public int curDeaths;
            public Timer curTimer;
            public Vector2 curCheckpoint;
            public List<ColorSwitcher.QColor> curColors;

            public override int TotalDeaths => totalDeaths;
            public override Timer TotalTimer => totalTimer;
            public override Timer RecordTimer => recordTimer;
            public override IReadOnlyCollection<int> Collectibles => collectibles;
            public override int CurDeaths => curDeaths;
            public override Timer CurTimer => curTimer;
            public override Vector2 CurCheckpoint => curCheckpoint;
            public override IReadOnlyCollection<ColorSwitcher.QColor> CurColors => curColors;

            public LevelStats(int slot, int level)
            {
                this.slot = slot;
                this.level = level;

                totalDeaths = PlayerPrefs.GetInt($"Slot{slot}_Level{level}TotalDeaths", 0);
                totalTimer = new Timer(PlayerPrefs.GetFloat($"Slot{slot}_Level{level}TotalTime", 0));
                recordTimer = new Timer(PlayerPrefs.GetFloat($"Slot{slot}_Level{level}RecordTime", float.PositiveInfinity));

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

                curDeaths = PlayerPrefs.GetInt($"Slot{slot}_Level{level}CurDeaths", 0);
                curTimer = new Timer(PlayerPrefs.GetFloat($"Slot{slot}_Level{level}CurTime", 0));
                curCheckpoint = new Vector2(
                    PlayerPrefs.GetFloat($"Slot{slot}_Level{level}CurCheckpointX", float.NegativeInfinity),
                    PlayerPrefs.GetFloat($"Slot{slot}_Level{level}CurCheckpointY", float.NegativeInfinity)
                );

                curColors = new List<ColorSwitcher.QColor>();
                flags = PlayerPrefs.GetInt($"Slot{slot}_Level{level}CurColors");
                foreach (ColorSwitcher.QColor value in (ColorSwitcher.QColor[])System.Enum.GetValues(typeof(ColorSwitcher.QColor)))
                {
                    if ((flags & (1 << (int)value)) > 0)
                    {
                        curColors.Add(value);
                    }
                }
            }

            public void ResetCurrent()
            {
                curDeaths = 0;
                curTimer.time = 0;
                curCheckpoint = Vector2.negativeInfinity;
            }

            public void Save()
            {
                PlayerPrefs.SetInt($"Slot{slot}_Level{level}TotalDeaths", TotalDeaths);
                PlayerPrefs.SetFloat($"Slot{slot}_Level{level}TotalTime", TotalTimer.time);
                PlayerPrefs.SetFloat($"Slot{slot}_Level{level}RecordTime", RecordTimer.time);

                //Makes an int32 out of ids, instead of 32 individual bools
                int flags = 0;
                foreach (int id in Collectibles)
                {
                    flags += 1 << id;
                }
                PlayerPrefs.SetInt($"Slot{slot}_Level{level}Collectibles", flags);

                PlayerPrefs.SetInt($"Slot{slot}_Level{level}CurDeaths", CurDeaths);
                PlayerPrefs.SetFloat($"Slot{slot}_Level{level}CurTime", CurTimer.time);
                PlayerPrefs.SetFloat($"Slot{slot}_Level{level}CurCheckpointX", CurCheckpoint.x);
                PlayerPrefs.SetFloat($"Slot{slot}_Level{level}CurCheckpointY", CurCheckpoint.y);

                flags = 0;
                foreach (ColorSwitcher.QColor value in CurColors)
                {
                    flags |= 1 << (int)value;
                }
                PlayerPrefs.SetInt($"Slot{slot}_Level{level}CurColors", flags);
            }

            public static void Clear(int slot, int level)
            {
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}TotalDeaths");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}TotalTime");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}RecordTime");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}Collectibles");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}CurDeaths");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}CurTime");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}CurCheckpointX");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}CurCheckpointY");
                PlayerPrefs.DeleteKey($"Slot{slot}_Level{level}CurColors");
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
            Instance.continueMode = enabled;
        }

        public static string GetLastLevelName()
        {
            return Instance.totalSlotStats?.curLevelName;
        }

        public static void ClearSaveSlot(int slot, LevelList levelList)
        {
            levelList = levelList ? levelList : LevelChanger.Instance?.LevelList;
            if (levelList)
            {
                TotalSlotStats.Clear(slot);
                for (int level = 0; level < levelList.levels.Count; level++)
                {
                    LevelStats.Clear(slot, level);
                }
                PlayerPrefs.Save();

                if (slot == Instance.saveSlot)
                {
                    SelectSaveSlot(slot);
                }
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
                Instance.levelStats.ResetCurrent();
            }

            return Instance.levelStats;
        }

        public static void SaveStats()
        {
            if (Instance.saveSlot == -1)
            {
                SelectSaveSlot(1);
            }
            Instance.totalSlotStats.Save();
            if (Instance.levelStats != null)
            {
                Instance.totalSlotStats.totalTimer.time += TimeCounter.Time.time - Instance.levelStats.curTimer.time;
                Instance.levelStats.curTimer.time = TimeCounter.Time.time;
                Instance.levelStats.Save();
            }

            PlayerPrefs.Save();
        }

        // ---------- Deaths ----------
        public static int GetDeaths()
        {
            return Instance.levelStats.curDeaths;
        }

        public static void AddDeath()
        {
            Instance.totalSlotStats.totalDeaths++;
            Instance.levelStats.totalDeaths++;
            Instance.levelStats.curDeaths++;
            SaveStats();
        }

        // ---------- Checkpoints ----------
        public static void SaveCheckpoint(Vector2 pos)
        {
            Instance.totalSlotStats.curLevelName = SceneManager.GetActiveScene().name;
            Instance.levelStats.curCheckpoint = pos;
            SaveStats();
        }

        public static void FinishLevel(string nextLevelName)
        {
            Instance.totalSlotStats.curLevelName = nextLevelName;
            Instance.levelStats.curCheckpoint = Vector2.negativeInfinity;

            if (Instance.levelStats.curTimer.time < Instance.levelStats.recordTimer.time)
            {
                Instance.levelStats.recordTimer.time = Instance.levelStats.curTimer.time;
            }
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
        
        public static int GetCollectiblesCount()
        {
            return Instance.levelStats.collectibles.Count;
        }

        public static bool SpendCollectibles(int amount)
        {
            if (Instance.levelStats.collectibles.Count >= amount)
            {
                int removed = 0;
                var toRemove = new List<int>();

                foreach (var id in Instance.levelStats.collectibles)
                {
                    toRemove.Add(id);
                    removed++;
                    if (removed >= amount)
                        break;
                }

                foreach (var id in toRemove)
                {
                    Instance.levelStats.collectibles.Remove(id);
                }

                SaveStats();
                return true;
            }
            return false;
        }

        // ---------- Colors ----------
        public static void PickUpColor(ColorSwitcher.QColor color)
        {
            Instance.levelStats.curColors.Add(color);
            SaveStats();
        }
    }
}
