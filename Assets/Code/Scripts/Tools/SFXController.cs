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

        /// <summary>
        /// Call change crystal sound event
        /// </summary>
        /// <param name="gameObject">Game Object</param>
        /// <param name="colour">New color</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ChangeToCrystal(GameObject gameObject, ColorSwitcher.QColour colour)
        {
            string colorEvent = colour switch
            {
                ColorSwitcher.QColour.None => "",
                ColorSwitcher.QColour.Red => RedEvent,
                ColorSwitcher.QColour.Blue => BlueEvent,
                ColorSwitcher.QColour.Green => GreenEvent,
                ColorSwitcher.QColour.Yellow => YellowEvent,
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