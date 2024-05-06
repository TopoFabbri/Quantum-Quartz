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
        public static event Action<Vector2> MoveCam;
        public static event Action Jump;
        public static event Action Color1;
        public static event Action Color2;
        public static event Action Color3;
        public static event Action Color4;
        public static event Action Restart;
        public static event Action Dash;
        public static event Action Djmp;

        private static bool _colorModifierPressed;

        /// <summary>
        /// Called when input move is pressed
        /// </summary>
        /// <param name="input">Input value</param>
        protected void OnMove(InputValue input)
        {
            Move?.Invoke(input.Get<Vector2>());
        }

        /// <summary>
        /// Called when camera movement is changed
        /// </summary>
        /// <param name="input">Input value</param>
        private void OnMoveCam(InputValue input)
        {
            MoveCam?.Invoke(input.Get<Vector2>());
        }

        /// <summary>
        /// Called when input jump is pressed
        /// </summary>
        protected void OnJump()
        {
            if (!_colorModifierPressed)
                Jump?.Invoke();
        }

        /// <summary>
        /// Call when change to color 1 input
        /// </summary>
        private void OnColor1()
        {
            if (_colorModifierPressed)
                Color1?.Invoke();
        }

        /// <summary>
        /// Call when change to color 2 input
        /// </summary>
        private void OnColor2()
        {
            if (_colorModifierPressed)
                Color2?.Invoke();
        }

        /// <summary>
        /// Call when change to color 3 input
        /// </summary>
        private void OnColor3()
        {
            if (_colorModifierPressed)
                Color3?.Invoke();
        }

        /// <summary>
        /// Call when change to color 4 input
        /// </summary>
        private void OnColor4()
        {
            if (_colorModifierPressed)
                Color4?.Invoke();
        }

        /// <summary>
        /// Set modifier pressed state
        /// </summary>
        /// <param name="input"></param>
        private void OnColorModifier(InputValue input)
        {
            _colorModifierPressed = input.isPressed;
        }

        /// <summary>
        /// Called when input reset is pressed
        /// </summary>
        private void OnRestart()
        {
            Restart?.Invoke();
        }

        /// <summary>
        /// Called when input dash is pressed
        /// </summary>
        private void OnDash()
        {
            if (!_colorModifierPressed)
                Dash?.Invoke();
        }

        /// <summary>
        /// Called when input MiniJump is pressed
        /// </summary>
        private void OnDjmp()
        {
            if (!_colorModifierPressed)
                Djmp?.Invoke();
        }
    }
}