namespace Code.Scripts.Level
{
    /// <summary>
    /// Timer for the level
    /// </summary>
    public static class TimeCounter
    {
        private static float _time;

        private static bool _count;

        private static int Hours => (int)_time / 3600;
        private static int Mins => (int)((_time - Hours * 3600) / 60);
        private static int Secs => (int)(_time - Hours * 3600 - Mins * 60) % 60;
        private static int MiliSecs => (int)(_time % 1f * 100f);
        public static string Time => Hours > 0 ? $"{Hours:00}:{Mins:00}:{Secs:00}:{MiliSecs:00}" : $"{Mins:00}:{Secs:00}:{MiliSecs:00}";
        
        public static void Update(float deltaTime)
        {
            if (!_count) return;
            
            _time += deltaTime;
        }
        
        /// <summary>
        /// Start timer
        /// </summary>
        public static void Start()
        {
            _count = true;
        }
        
        /// <summary>
        /// Stop timer
        /// </summary>
        public static void Stop()
        {
            _count = false;
        }
        
        /// <summary>
        /// Reset timer
        /// </summary>
        public static void Reset()
        {
            _time = 0f;
        }
    }
}