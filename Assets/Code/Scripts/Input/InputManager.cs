using Code.Scripts.Colors;
using Code.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Input
{
    /// <summary>
    /// Cast input events
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public abstract class ReadOnlyInputMap
        {
            public abstract string GetMapName();
            public abstract bool GetContextualPower();
            public abstract bool GetDoubleClickPower();
        }

        [System.Serializable]
        class InputMap : ReadOnlyInputMap
        {
            public string mapName;
            public bool contextualPower;
            public bool doubleClickPower;
            public override string GetMapName() => mapName;
            public override bool GetContextualPower() => contextualPower;
            public override bool GetDoubleClickPower() => doubleClickPower;
        }

        public static event Action<Vector2> Move;
        public static event Action<float> Jump;
        public static event Action ColorRed;
        public static event Action ColorBlue;
        public static event Action ColorGreen;
        public static event Action ColorYellow;
        public static event Action Restart;
        public static event Action AbilityPress;
        public static event Action AbilityRelease; 
        public static event Action Pause;
        public static event Action DevMode;

        [SerializeField] private float moveDeadzone = .5f;
        [SerializeField] private GameObject eventSystem;

        public static PlayerInput Input { get; private set; }
        public static ReadOnlyInputMap activeMap;

        [SerializeField, Disable]
        private string gameMap = "World";
        private string uiMap = "UI";
        private int inputMapIndex = 0;

        [SerializeField] private List<InputMap> inputMaps;

        [HeaderPlus("Debug")]
        [InspectorButton("Next Input Map")]
        private void NextInputMap()
        {
            SwitchGameMap(1);
        }

        [InspectorButton("Previous Input Map")]
        private void PreviousInputMap()
        {
            SwitchGameMap(-1);
        }

        private void OnEnable()
        {
            Input = GetComponent<PlayerInput>();
            string controlsMapping = PlayerPrefs.GetString("ControlsMapping", "");
            if (string.IsNullOrWhiteSpace(controlsMapping))
            {
                SwitchGameMap(0);
            }
            else
            {
                SwitchGameMap(controlsMapping);
            }
        }
        
        private void OnDisable()
        {
            Input = null;
        }

        public void EnableGameMap()
        {
            eventSystem.SetActive(false);
            Input.SwitchCurrentActionMap(gameMap);
        }

        public void EnableUIMap()
        {
            eventSystem.SetActive(true);
            Input.SwitchCurrentActionMap(uiMap);
        }

        public void SwitchGameMap(int offset)
        {
            int newIndex = Mathf.FloorToInt(Mathf.Repeat(inputMapIndex + offset, inputMaps.Count));
            SwitchGameMap(inputMaps[newIndex].mapName);
        }

        public void SwitchGameMap(string mapName)
        {
            InputActionMap map = Input.actions.FindActionMap(mapName, false);
            if (map == null)
            {
                Debug.LogError("Input map '" + mapName + "' does not exist");
                return;
            }

            int newIndex = -1;
            for (int i = 0; i < inputMaps.Count; i++)
            {
                if (inputMaps[i].mapName.Equals(mapName))
                {
                    newIndex = i;
                    break;
                }
            }

            if (newIndex < 0)
            {
                Debug.LogError("Input map '" + mapName + "' is not listed in InputManager's list of input maps");
                return;
            }

            bool inGame = Input.currentActionMap.name.Equals(gameMap);
            gameMap = mapName;
            inputMapIndex = newIndex;
            activeMap = inputMaps[inputMapIndex];
            if (inGame)
            {
                EnableGameMap();
            }
        }

        /// <summary>
        /// Called when input move is pressed
        /// </summary>
        /// <param name="input">Input value</param>
        protected void OnMove(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();

            Move?.Invoke(value.magnitude > moveDeadzone ? value : Vector2.zero);
        }

        /// <summary>
        /// Called when input jump is pressed
        /// </summary>
        protected void OnJump(InputValue input)
        {
            if (input.Get<float>() != 0)
            {
                Jump?.Invoke(1);
            }
            else if (activeMap.GetContextualPower())
            {
                Jump?.Invoke(0);
            }
        }

        /// <summary>
        /// Call when change to color red input
        /// </summary>
        private void OnColorRed(InputValue input)
        {
            if (activeMap.GetDoubleClickPower() && ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Red)
            {
                OnAbility(input);
            }
            else
            {
                ColorRed?.Invoke();
            }
        }

        /// <summary>
        /// Call when change to color blue input
        /// </summary>
        private void OnColorBlue(InputValue input)
        {
            if (activeMap.GetDoubleClickPower() && ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Blue)
            {
                OnAbility(input);
            }
            else
            {
                ColorBlue?.Invoke();
            }
        }

        /// <summary>
        /// Call when change to color green input
        /// </summary>
        private void OnColorGreen(InputValue input)
        {
            if (activeMap.GetDoubleClickPower() && ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Green)
            {
                OnAbility(input);
            }
            else
            {
                ColorGreen?.Invoke();
            }
        }

        /// <summary>
        /// Call when change to color yellow input
        /// </summary>
        private void OnColorYellow(InputValue input)
        {
            if (activeMap.GetDoubleClickPower() && ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Yellow)
            {
                OnAbility(input);
            }
            else
            {
                ColorYellow?.Invoke();
            }
        }

        /// <summary>
        /// Called when input reset is pressed
        /// </summary>
        private void OnRestart()
        {
            if (!Application.isEditor)
                return;

            Restart?.Invoke();
        }

        /// <summary>
        /// Called when input ability is pressed
        /// </summary>
        private void OnAbility(InputValue input)
        {
            float value = input.Get<float>();
            
            if (value != 0)
                AbilityPress?.Invoke();
            else
                AbilityRelease?.Invoke();
        }

        /// <summary>
        /// Called when input pause is pressed
        /// </summary>
        private void OnPause()
        {
            Pause?.Invoke();
        }

        /// <summary>
        /// Called when input dev mode is pressed
        /// </summary>
        private void OnDevMode()
        {
            if (!Application.isEditor)
                return;
            
            DevMode?.Invoke();
        }

        private void OnColorRedAbility(InputValue input)
        {
            if (!ColorSwitcher.Instance.EnabledColours.Contains(ColorSwitcher.QColour.Red))
                return;

            if (input.Get<float>() != 0)
            {
                ColorRed?.Invoke();
            }
            OnAbility(input);
        }

        private void OnColorBlueAbility(InputValue input)
        {
            if (!ColorSwitcher.Instance.EnabledColours.Contains(ColorSwitcher.QColour.Blue))
                return;

            if (input.Get<float>() != 0)
            {
                ColorBlue?.Invoke();
            }
            OnAbility(input);
        }

        private void OnColorGreenAbility(InputValue input)
        {
            if (!ColorSwitcher.Instance.EnabledColours.Contains(ColorSwitcher.QColour.Green))
                return;

            if (input.Get<float>() != 0)
            {
                ColorGreen?.Invoke();
            }
            OnAbility(input);
        }

        private void OnColorYellowAbility(InputValue input)
        {
            if (!ColorSwitcher.Instance.EnabledColours.Contains(ColorSwitcher.QColour.Yellow))
                return;

            if (input.Get<float>() != 0)
            {
                ColorYellow?.Invoke();
            }
            OnAbility(input);
        }
    }
}