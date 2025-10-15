using UnityEngine;

namespace Code.Scripts.Game.Managers
{
    public class Settings
    {
        private static Settings _instance;

        public bool devMode;
        private float musicVol = 100f;
        private float sfxVol = 100f;
        private bool contextualBlue = false;
        private bool colorFreeze = false;
        private bool showGameTimer = false;

        public static Settings Instance
        {
            get
            {
                _instance ??= new Settings();

                return _instance;
            }
        }

        public Settings()
        {
            musicVol = PlayerPrefs.GetFloat("MusicVolume", musicVol);
            sfxVol = PlayerPrefs.GetFloat("SfxVolume", sfxVol);
            contextualBlue = PlayerPrefs.GetInt("ContextualBlue", contextualBlue ? 1 : 0) == 1;
            colorFreeze = PlayerPrefs.GetInt("ColorFreeze", colorFreeze ? 1 : 0) == 1;
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

        public static bool ContextualBlue
        {
            get => Instance.contextualBlue;
            set
            {
                Instance.contextualBlue = value;
                PlayerPrefs.SetInt("ContextualBlue", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static bool ColorFreeze
        {
            get => Instance.colorFreeze;
            set
            {
                Instance.colorFreeze = value;
                PlayerPrefs.SetInt("ColorFreeze", value ? 1 : 0);
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