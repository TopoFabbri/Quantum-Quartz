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
        [System.Serializable]
        struct InputMap
        {
            public string mapName;
            public bool contextualPower;
            public bool doubleClickPower;
        }

        public static event Action<Vector2> Move;
        public static event Action Jump;
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
        [SerializeField] private List<InputMap> inputMaps;
        private int inputMapIndex = 0;

        public static PlayerInput Input { get; private set; }
        private string gameMap = "World";
        private string uiMap = "UI";

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
            SwitchGameMap(0);
        }
        
        private void OnDisable()
        {
            Input = null;
        }

        public void EnableGameMap()
        {
            Input.SwitchCurrentActionMap(gameMap);
        }

        public void EnableUIMap()
        {
            Input.SwitchCurrentActionMap(uiMap);
        }

        public void SwitchGameMap(int offset)
        {
            inputMapIndex = Mathf.FloorToInt(Mathf.Repeat(inputMapIndex + offset, inputMaps.Count));
            SwitchGameMap(inputMaps[inputMapIndex].mapName);
        }

        public void SwitchGameMap(string mapName)
        {
            InputActionMap map = Input.actions.FindActionMap(mapName, false);
            if (map == null)
            {
                Debug.LogError("Input map '" + mapName + "' does not exist");
                return;
            }

            bool inList = false;
            foreach (InputMap inputMap in inputMaps)
            {
                if (inputMap.mapName.Equals(mapName))
                {
                    inList = true;
                    // Here the boolean settings should be processed
                    break;
                }
            }

            if (!inList)
            {
                Debug.LogError("Input map '" + mapName + "' is not listed in InputManager's list of input maps");
                return;
            }

            bool inGame = Input.currentActionMap.name.Equals(gameMap);
            gameMap = mapName;
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
        protected void OnJump()
        {
            Jump?.Invoke();
        }

        /// <summary>
        /// Call when change to color red input
        /// </summary>
        private void OnColorRed()
        {
            ColorRed?.Invoke();
        }

        /// <summary>
        /// Call when change to color blue input
        /// </summary>
        private void OnColorBlue()
        {
            ColorBlue?.Invoke();
        }

        /// <summary>
        /// Call when change to color green input
        /// </summary>
        private void OnColorGreen()
        {
            ColorGreen?.Invoke();
        }

        /// <summary>
        /// Call when change to color yellow input
        /// </summary>
        private void OnColorYellow()
        {
            ColorYellow?.Invoke();
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
            if (!ColorSwitcher.EnabledColours.Contains(ColorSwitcher.QColour.Red))
                return;

            float value = input.Get<float>();

            if (value != 0)
            {
                ColorRed?.Invoke();
                AbilityPress?.Invoke();
            }
            else
            {
                AbilityRelease?.Invoke();
            }
        }

        private void OnColorBlueAbility(InputValue input)
        {
            if (!ColorSwitcher.EnabledColours.Contains(ColorSwitcher.QColour.Blue))
                return;

            float value = input.Get<float>();

            if (value != 0)
            {
                ColorBlue?.Invoke();
                AbilityPress?.Invoke();
            }
            else
            {
                AbilityRelease?.Invoke();
            }
        }

        private void OnColorGreenAbility(InputValue input)
        {
            if (!ColorSwitcher.EnabledColours.Contains(ColorSwitcher.QColour.Green))
                return;

            float value = input.Get<float>();

            if (value != 0)
            {
                ColorGreen?.Invoke();
                AbilityPress?.Invoke();
            }
            else
            {
                AbilityRelease?.Invoke();
            }
        }

        private void OnColorYellowAbility(InputValue input)
        {
            if (!ColorSwitcher.EnabledColours.Contains(ColorSwitcher.QColour.Yellow))
                return;

            float value = input.Get<float>();

            if (value != 0)
            {
                ColorYellow?.Invoke();
                AbilityPress?.Invoke();
            }
            else
            {
                AbilityRelease?.Invoke();
            }
        }
    }
}