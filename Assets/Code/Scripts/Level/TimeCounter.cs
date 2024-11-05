namespace Code.Scripts.Level
{
    /// <summary>
    /// Timer for the level
    /// </summary>
    public static class TimeCounter
    {
        private static bool _count;

        public static Timer Time;
        
        public static void Update(float deltaTime)
        {
            if (!_count) return;
            
            Time.time += deltaTime;
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
            Time.time = 0f;
        }
    }
}