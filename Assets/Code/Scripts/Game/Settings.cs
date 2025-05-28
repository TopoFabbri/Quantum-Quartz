using AK.Wwise;

namespace Code.Scripts.Game
{
    public class Settings
    {
        private static Settings _instance;

        public bool devMode;
        private float musicVol = 100f;
        private float sfxVol = 100f;

        public static Settings Instance => _instance ??= new Settings();

        public static float MusicVol
        {
            get => Instance.musicVol;
            set
            {
                Instance.musicVol = value;
                AkSoundEngine.SetRTPCValue("RTPC_MusicVolume", value);
            }
        }
        
        public static float SfxVol
        {
            get => Instance.sfxVol;
            set
            {
                Instance.sfxVol = value;
                AkSoundEngine.SetRTPCValue("RTPC_SfxVolume", value);
            }
        }
    }
}