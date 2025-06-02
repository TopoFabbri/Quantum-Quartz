
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
        public bool Falling { get; private set; }
        public bool IsGrounded { get; private set; }
        public Type PreviousStateType => stateMachine.PreviousState?.GetType();
        public Type CurrentStateType => stateMachine.CurrentState?.GetType();

        public bool facingRight = false;
        public bool died = false;
        public bool canCoyoteJump = false;
        public Vector2 speed;
        public float jumpFallTime = 0;

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
        /// Check if player is on ground, and update IsGrounded
        /// </summary>
        /// <returns>True if on the ground</returns>
        public bool RecalculateIsGrounded()
        {
            bool grounded = false;

            if (Rigidbody.velocity.y == 0 || GlobalSettings.shouldDraw)
            {
                Vector2 startPos = (Vector2)Transform.position + GlobalSettings.groundCheckOffset;
                grounded = HitFloor(Physics2D.RaycastAll(startPos, Vector2.down, GlobalSettings.groundCheckRadius, GlobalSettings.groundLayer));

                if (GlobalSettings.shouldDraw)
                {
                    Debug.DrawLine(startPos, startPos + Vector2.down * GlobalSettings.groundCheckRadius, grounded ? Color.green : Color.red);
                }

                if (!grounded || GlobalSettings.shouldDraw)
                {
                    grounded |= GetEdge(true);

                    if (!grounded || GlobalSettings.shouldDraw)
                    {
                        grounded |= GetEdge(false);
                        grounded &= Rigidbody.velocity.y == 0;
                    }
                }
            }

            IsGrounded = grounded;
            return IsGrounded;
        }

        bool GetEdge(bool right)
        {
            Vector2 startPos = (Vector2)Transform.position + GlobalSettings.groundCheckOffset + (right ? Vector2.right : Vector2.left) * GlobalSettings.edgeCheckDis;
            bool onEdge = HitFloor(Physics2D.RaycastAll(startPos, Vector2.down, GlobalSettings.edgeCheckLength, GlobalSettings.groundLayer));

            if (GlobalSettings.shouldDraw)
            {
                Debug.DrawLine(startPos, startPos + Vector2.down * GlobalSettings.edgeCheckLength, onEdge ? Color.green : Color.red);
            }

            return onEdge;
        }

        private bool HitFloor(RaycastHit2D[] hits)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.distance > 0 && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform")))
                {
                    return true;
                }
            }
            return false;
        }

        public void SetFalling(bool falling)
        {
            Falling = falling;
            jumpFallTime = 0;
        }
    }
}
