using AK.Wwise;
using Code.Scripts.Game.Triggers;
using Code.Scripts.Input;
using Code.Scripts.Player;
using Code.Scripts.Tools;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Game.Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private TextMeshProUGUI timerTxt;
        [SerializeField] private GameObject statesText;

        [SerializeField] private DroneController drone;
        [SerializeField] private PlayerController player;
        [SerializeField] private Switch musicState;
        [SerializeField] private Event musicEvent;

        bool framerateInputs = false;

        public DroneController Drone => drone;
        public PlayerController Player => player;

        private void Start()
        {
#if INPUT_LAG
            Application.targetFrameRate = 20;
#else
            Application.targetFrameRate = 60;
#endif
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            TimeCounter.Start();
            TimeCounter.Reset();

            Stats.ReadOnlyLevelStats levelStats = Stats.LoadLevelStats(LevelChanger.Instance.CurrentLevel);

            TimeCounter.Time.time = levelStats.CurTimer.time;
            if (!Vector2.negativeInfinity.Equals(levelStats.CurCheckpoint))
            {
                player.SpawnAt(levelStats.CurCheckpoint);
            }
            else
            {
                player.SpawnAt(player.transform.position);
            }

            foreach (ColorSwitcher.QColor color in new List<ColorSwitcher.QColor>(levelStats.CurColors))
            {
                if (!ColorSwitcher.Instance.EnabledColors.Contains(color))
                {
                    ColorSwitcher.Instance.EnableColor(color);
                }
            }

            timerTxt?.gameObject.SetActive(Settings.ShowGameTimer);

            SfxController.MusicOnOff(true, gameObject, musicEvent);
            SfxController.SetMusicState(musicState, gameObject);
        }

        private void OnEnable()
        {
            InputManager.Restart += OnRestartHandler;
            InputManager.DevMode += OnDevModeHandler;

        }

        private void OnDisable()
        {
            InputManager.Restart -= OnRestartHandler;
            InputManager.DevMode -= OnDevModeHandler;
            Stats.SaveStats();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SfxController.StopAllOn(gameObject);
            Stats.SaveStats();
        }

        private void OnApplicationQuit()
        {
            Stats.SaveStats();
        }

        private void Update()
        {
            TimeCounter.Update(Time.deltaTime);
            if (timerTxt)
            {
                timerTxt.text = TimeCounter.Time.ToStr;
            }

            if (!framerateInputs && (Time.timeScale == 0 || Time.timeScale / Time.fixedDeltaTime < 60))
            {
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
                framerateInputs = true;
            }
            else if (framerateInputs && Time.timeScale > 0 && Time.timeScale / Time.fixedDeltaTime > 60)
            {
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
                framerateInputs = false;
            }
        }

        private void OnDevModeHandler()
        {
            Settings.Instance.devMode = !Settings.Instance.devMode;

            statesText.SetActive(Settings.Instance.devMode);
            Settings.MusicVol = Settings.Instance.devMode ? 0 : 100f;
        }

        private static void OnRestartHandler()
        {
            LevelChanger.Instance.ReloadLevel();
        }

        public void SetMusicFaded(bool faded)
        {
            SfxController.SetMusicFaded(faded, gameObject);
        }
    }
}