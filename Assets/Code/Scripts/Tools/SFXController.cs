using System;
using Code.Scripts.Colors;
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
        private const string SpringEvent = "Play_Steam_Spring";

        private const string BlueEvent = "Play_Change_Quartz_B";
        private const string RedEvent = "Play_Change_Quartz_R";
        private const string YellowEvent = "Play_Change_Quartz_Y";
        private const string GreenEvent = "Play_Change_Quartz_G";

        public static GameObject musicObject;
        
        /// <summary>
        /// Call music sound event
        /// </summary>
        /// <param name="on"></param>
        /// <param name="gameObject"></param>
        public static void MusicOnOff(bool on, GameObject gameObject)
        {
            if (on)
            {
                AkSoundEngine.PostEvent(PlayMusicEvent, gameObject);
                musicObject = gameObject;
            }
            else
            {
                AkSoundEngine.PostEvent(StopMusicEvent, gameObject);
                musicObject = null;
            }
        }

        /// <summary>
        /// Call blocked crystal sound event
        /// </summary>
        public static void BlockedCrystal(GameObject gameObject)
        {
            AkSoundEngine.PostEvent(BlockedCrystalEvent, gameObject);
        }

        /// <summary>
        /// Call change crystal sound event
        /// </summary>
        /// <param name="gameObject">Game Object</param>
        /// <param name="colour">New color</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ChangeToCrystal(GameObject gameObject, ColorSwitcher.QColor colour)
        {
            string colorEvent = colour switch
            {
                ColorSwitcher.QColor.None => "",
                ColorSwitcher.QColor.Red => RedEvent,
                ColorSwitcher.QColor.Blue => BlueEvent,
                ColorSwitcher.QColor.Green => GreenEvent,
                ColorSwitcher.QColor.Yellow => YellowEvent,
                _ => throw new ArgumentOutOfRangeException(nameof(colour), colour, null)
            };

            if (!string.IsNullOrEmpty(colorEvent))
                AkSoundEngine.PostEvent(colorEvent, gameObject);
        }
        
        public static void PlaySpring(GameObject go)
        {
            AkSoundEngine.PostEvent(SpringEvent, go);
        }

        public static void StopAllOn(GameObject go)
        {
            if (!AkSoundEngine.IsGameObjectRegistered(go))
                return;
            
            AkSoundEngine.StopAll(go);
        }
        
    }
}