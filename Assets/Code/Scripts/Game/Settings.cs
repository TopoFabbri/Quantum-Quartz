using AK.Wwise;
using UnityEngine;

namespace Code.Scripts.Game
{
    public class Settings
    {
        private static Settings _instance;

        public bool devMode;
        private float musicVol = 100f;
        private float sfxVol = 100f;
        private bool showGameTimer = false;

        public static Settings Instance => _instance != null ? _instance : new Settings();

        public Settings()
        {
            musicVol = PlayerPrefs.GetFloat("MusicVolume", musicVol);
            sfxVol = PlayerPrefs.GetFloat("SfxVolume", sfxVol);
            showGameTimer = PlayerPrefs.GetInt("Timer", showGameTimer ? 1 : 0) == 1;
            AkSoundEngine.SetRTPCValue("RTPC_MusicVolume", musicVol);
            AkSoundEngine.SetRTPCValue("RTPC_SfxVolume", sfxVol);
        }

        public static float MusicVol
        {
            get => Instance.musicVol;
            set
            {
                Instance.musicVol = value;
                AkSoundEngine.SetRTPCValue("RTPC_MusicVolume", value);
                PlayerPrefs.SetFloat("MusicVolume", value);
                PlayerPrefs.Save();
            }
        }
        
        public static float SfxVol
        {
            get => Instance.sfxVol;
            set
            {
                Instance.sfxVol = value;
                AkSoundEngine.SetRTPCValue("RTPC_SfxVolume", value);
                PlayerPrefs.SetFloat("SfxVolume", value);
                PlayerPrefs.Save();
            }
        }

        public static bool ShowGameTimer
        {
            get => Instance.showGameTimer;
            set
            {
                Instance.showGameTimer = value;
                PlayerPrefs.SetInt("Timer", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
    }
}