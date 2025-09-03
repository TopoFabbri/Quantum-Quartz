using AK.Wwise;
using Code.Scripts.Input;
using Code.Scripts.Level;
using Code.Scripts.Player;
using Code.Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Game
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private TextMeshProUGUI timerTxt;
        [SerializeField] private GameObject statesText;

        [SerializeField] private DroneController drone;
        [SerializeField] private PlayerController player;
        [SerializeField] private Switch musicState;
        [SerializeField] private Event musicEvent;
        
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

        private void OnDestroy()
        {
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
            timerTxt.text = TimeCounter.Time.ToStr;
        }

        private void OnDevModeHandler()
        {
            Settings.Instance.devMode = !Settings.Instance.devMode;

            statesText.SetActive(Settings.Instance.devMode);
            Settings.MusicVol = Settings.Instance.devMode ? 0 : 100f;
        }

        private static void OnRestartHandler()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}