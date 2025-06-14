namespace Code.Scripts.Level
{
    public struct Timer
    {
        public float time;

        public int Hours => (int)time / 3600;
        public int Mins => (int)((time - Hours * 3600) / 60);
        public int Secs => (int)(time - Hours * 3600 - Mins * 60) % 60;
        public int MiliSecs => (int)(time % 1f * 100f);
        public string ToStr => Hours > 0 ? $"{Hours:00}:{Mins:00}:{Secs:00}:{MiliSecs:00}" : $"{Mins:00}:{Secs:00}:{MiliSecs:00}";

        public Timer(float time)
        {
            this.time = time;
        }
    }
}