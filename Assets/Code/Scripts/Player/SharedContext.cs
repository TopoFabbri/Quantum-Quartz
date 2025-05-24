
using Code.Scripts.Camera;
using Code.Scripts.FSM;
using Code.Scripts.Input;
using Code.Scripts.States;
using Code.Scripts.StateSettings;
using System;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class SharedContext
    {
        public Rigidbody2D Rigidbody { get; private set; }
        public Transform Transform { get; private set; }
        public MonoBehaviour MonoBehaviour { get; private set; }
        public PlayerSfx PlayerSfx { get; private set; }
        public GlobalSettings GlobalSettings { get; private set; }
        public CameraController CamController { get; private set; }
        public float Input { get; private set; }
        public Type PreviousStateType => stateMachine.PreviousState?.GetType();
        public Type CurrentStateType => stateMachine.CurrentState?.GetType();

        public bool facingRight = false;
        public bool falling = false;
        public bool died = false;
        public bool canCoyoteJump = false;

        private readonly FiniteStateMachine<string> stateMachine;

        public SharedContext(Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx, GlobalSettings globalSettings, FiniteStateMachine<string> stateMachine)
        {
            Rigidbody = rb;
            Transform = transform;
            MonoBehaviour = mb;
            PlayerSfx = playerSfx;
            GlobalSettings = globalSettings;
            this.stateMachine = stateMachine;

            InputManager.Move += OnMoveHandler;

            if (UnityEngine.Camera.main?.transform.parent != null)
            {
                UnityEngine.Camera.main.transform.parent.TryGetComponent(out CameraController camController);
                CamController = camController;
            }
        }

        ~SharedContext()
        {
            InputManager.Move -= OnMoveHandler;
        }

        /// <summary>
        /// Handle player move action
        /// </summary>
        /// <param name="input">Input value</param>
        void OnMoveHandler(Vector2 input)
        {
            Input = input.x;
        }

        /// <summary>
        /// Check if player is on ground
        /// </summary>
        /// <returns>True if on the ground</returns>
        public bool IsGrounded()
        {
            Vector2 pos = Transform.position;
            RaycastHit2D hit = Physics2D.Raycast(pos + GlobalSettings.groundCheckOffset, Vector2.down, GlobalSettings.groundCheckRadius, GlobalSettings.groundLayer);
            bool grounded = hit.collider && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform"));

            if (GlobalSettings.shouldDraw)
            {
                Debug.DrawLine(
                    pos + GlobalSettings.groundCheckOffset,
                    pos + GlobalSettings.groundCheckOffset + Vector2.down * GlobalSettings.groundCheckRadius,
                    grounded ? Color.green : Color.red
                );
                Debug.DrawLine(
                    pos + GlobalSettings.groundCheckOffset,
                    pos + GlobalSettings.groundCheckOffset + Vector2.down * GlobalSettings.groundCheckRadius,
                    grounded ? Color.green : Color.red
                );
            }
            return grounded || ((stateMachine.CurrentState as EdgeState<string>)?.IsOnEdge() == true);
        }
    }
}
