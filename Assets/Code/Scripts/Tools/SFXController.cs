using UnityEngine;

namespace Code.Scripts.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public static class SfxController
    {
        private const string PlayMusicEvent = "Play_Steampunk_Music";
        private const string StopMusicEvent = "Stop_Steampunk_Music";
        private const string BlockedCrystalEvent = "Play_Blocked_Quartz";

        /// <summary>
        /// Call music sound event
        /// </summary>
        /// <param name="on"></param>
        /// <param name="gameObject"></param>
        public static void MusicOnOff(bool on, GameObject gameObject)
        {
            AkSoundEngine.PostEvent(on ? PlayMusicEvent : StopMusicEvent, gameObject);
        }

        /// <summary>
        /// Call blocked crystal sound event
        /// </summary>
        public static void BlockedCrystal(GameObject gameObject)
        {
            AkSoundEngine.PostEvent(BlockedCrystalEvent, gameObject);
        }
    }
}
