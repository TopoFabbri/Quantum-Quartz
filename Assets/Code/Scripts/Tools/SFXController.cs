using System;
using AK.Wwise;
using Code.Scripts.Colors;
using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public static class SfxController
    {
        private const string PlayMusicEvent = "Play_MX_Switch";
        private const string StopMusicEvent = "Set_MXSwitch_NoMX";
        private const string BlockedCrystalEvent = "Play_Blocked_Quartz";
        private const string SpringEvent = "Play_Steam_Spring";

        private const string BlueEvent = "Play_Change_Quartz_B";
        private const string RedEvent = "Play_Change_Quartz_R";
        private const string YellowEvent = "Play_Change_Quartz_Y";
        private const string GreenEvent = "Play_Change_Quartz_G";

        public static GameObject musicObject;
        public static event Action<string> MusicCue;

        private static uint _musicEventId;
        
        /// <summary>
        /// Call music sound event
        /// </summary>
        /// <param name="on"></param>
        /// <param name="gameObject"></param>
        public static void MusicOnOff(bool on, GameObject gameObject, Event musicEvent)
        {
            if (on)
            {
                _musicEventId = musicEvent.Post(gameObject, (uint)(AkCallbackType.AK_MusicSyncBar | AkCallbackType.AK_MusicSyncBeat | AkCallbackType.AK_MusicSyncUserCue),
                    MusicCallbackFunction);
                musicObject = gameObject;
            }
            else
            {
                AkSoundEngine.PostEvent(StopMusicEvent, gameObject);
                musicObject = null;
            }
        }

        public static void SetMusicState(Switch stateEvent, GameObject gameObject)
        {
            stateEvent.SetValue(gameObject);
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

        private static void MusicCallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
        {
            AkMusicSyncCallbackInfo musicSyncInfo = (AkMusicSyncCallbackInfo)in_info;

            if (in_type == AkCallbackType.AK_MusicSyncUserCue)
            {
                Debug.Log("User Cue '" + musicSyncInfo.userCueName + "' reached!");
                MusicCue?.Invoke(musicSyncInfo.userCueName);
            }
        }
    }
}