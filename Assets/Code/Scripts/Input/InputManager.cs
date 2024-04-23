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
        public static event Action<float> ChangeColor;
        public static event Action Color1;
        public static event Action Color2;
        public static event Action Color3;
        public static event Action Color4;

        /// <summary>
        /// Called when input move is pressed
        /// </summary>
        /// <param name="input">Input value</param>
        protected void OnMove(InputValue input)
        {
            Move?.Invoke(input.Get<Vector2>());
        }

        /// <summary>
        /// Called when input jump is pressed
        /// </summary>
        protected void OnJump()
        {
            Jump?.Invoke();
        }

        /// <summary>
        /// Call when color changer input is pressed
        /// </summary>
        /// <param name="obj">Button value</param>
        private static void OnChangeColor(float obj)
        {
            ChangeColor?.Invoke(obj);
        }
        
        /// <summary>
        /// Call when change to color 1 input
        /// </summary>
        private static void OnColor1()
        {
            Color1?.Invoke();
        }

        /// <summary>
        /// Call when change to color 2 input
        /// </summary>
        private static void OnColor2()
        {
            Color2?.Invoke();
        }

        /// <summary>
        /// Call when change to color 3 input
        /// </summary>
        private static void OnColor3()
        {
            Color3?.Invoke();
        }

        /// <summary>
        /// Call when change to color 4 input
        /// </summary>
        private static void OnColor4()
        {
            Color4?.Invoke();
        }
    }
}
