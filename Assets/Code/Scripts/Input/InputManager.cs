using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Input
{
    /// <summary>
    /// Cast input events
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static event Action<Vector2> Move;
        public static event Action Jump;
        public static event Action Color1;
        public static event Action Color2;
        public static event Action Color3;
        public static event Action Color4;
        public static event Action Restart;
        public static event Action AbilityPress;
        public static event Action AbilityRelease; 
        public static event Action Pause;
        public static event Action DevMode;

        [SerializeField] private float moveDeadzone = .5f;

        public static PlayerInput Input { get; private set; }
        private string gameMap = "World";
        private string uiMap = "UI";

        private void OnEnable()
        {
            Input = GetComponent<PlayerInput>();
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

        public void SwitchGameMap(InputActionMap map)
        {
            gameMap = map.name;
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
        /// Call when change to color 1 input
        /// </summary>
        private void OnColor1()
        {
            Color1?.Invoke();
        }

        /// <summary>
        /// Call when change to color 2 input
        /// </summary>
        private void OnColor2()
        {
            Color2?.Invoke();
        }

        /// <summary>
        /// Call when change to color 3 input
        /// </summary>
        private void OnColor3()
        {
            Color3?.Invoke();
        }

        /// <summary>
        /// Call when change to color 4 input
        /// </summary>
        private void OnColor4()
        {
            Color4?.Invoke();
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

        private void OnColor1Power(InputValue input)
        {
            float value = input.Get<float>();

            if (value != 0)
            {
                Color1?.Invoke();
                AbilityPress?.Invoke();
            }
            else
            {
                AbilityRelease?.Invoke();
            }
        }

        private void OnColor2Power(InputValue input)
        {
            float value = input.Get<float>();

            if (value != 0)
            {
                Color2?.Invoke();
                AbilityPress?.Invoke();
            }
            else
            {
                AbilityRelease?.Invoke();
            }
        }

        private void OnColor3Power(InputValue input)
        {
            float value = input.Get<float>();

            if (value != 0)
            {
                Color3?.Invoke();
                AbilityPress?.Invoke();
            }
            else
            {
                AbilityRelease?.Invoke();
            }
        }

        private void OnColor4Power(InputValue input)
        {
            float value = input.Get<float>();

            if (value != 0)
            {
                Color4?.Invoke();
                AbilityPress?.Invoke();
            }
            else
            {
                AbilityRelease?.Invoke();
            }
        }
    }
}