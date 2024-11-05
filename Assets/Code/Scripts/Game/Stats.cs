using Code.Scripts.Level;

namespace Code.Scripts.Game
{
    /// <summary>
    /// Class to save game stats
    /// </summary>
    public class Stats
    {
        private static Stats _instance;

        private Timer time;
        
        private static Stats Instance => _instance ??= new Stats();
        
        public static Timer Time => Instance.time;

        /// <summary>
        /// Set last run's time
        /// </summary>
        /// <param name="time"> Value to set</param>
        public static void SetTime(Timer time)
        {
            Instance.time = time;
        }
    }
}
