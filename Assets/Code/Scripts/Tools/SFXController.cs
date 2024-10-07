using UnityEngine;

namespace Code.Scripts.Tools
{
    public static class SfxController
    {
        private const string PlayMusicEvent = "Play_Steampunk_Music";
        private const string StopMusicEvent = "Stop_Steampunk_Music";

        public static void MusicOnOff(bool on, GameObject gameObject)
        {
            AkSoundEngine.PostEvent(on ? PlayMusicEvent : StopMusicEvent, gameObject);
        }
    }
}
