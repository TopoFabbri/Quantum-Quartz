using System.Collections;
using System.Linq;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Jump up state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JumpState<T> : MoveState<T>
    {
        protected JumpSettings JumpSettings => settings as JumpSettings;

        protected readonly MonoBehaviour mb;
        
        public bool HasJumped { get; protected set; }
        public float JumpBufferTime => JumpSettings.bufferTime;

        public JumpState(T id, StateSettings.StateSettings stateSettings, MonoBehaviour mb, Rigidbody2D rb,
            Transform transform) : base(id, stateSettings, rb, transform)
        {
            settings = stateSettings;
            moveSettings = JumpSettings.moveSettings;

            this.mb = mb;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            mb.StartCoroutine(JumpOnFU());

            rb.sharedMaterial.friction = JumpSettings.moveSettings.airFriction;
        }

        public override void OnExit()
        {
            base.OnExit();

            HasJumped = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!IsGrounded())
                HasJumped = true;
            
        }
        
        protected void ApplyJumpForce()
        {
            rb.AddForce(JumpSettings.jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
        
        public float TryJump(float coyoteTimeRemaining)
        {
            if (coyoteTimeRemaining > 0f)
            {
                ApplyJumpForce();
                // Reset coyote time
                return 0f;
            }

            // If coyote time is not active, return the original value
            return coyoteTimeRemaining;
        }

        /// <summary>
        /// Wait for fixed update and jump
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator JumpOnFU()
        {
            yield return new WaitForFixedUpdate();
            
            //rb.AddForce(JumpSettings.jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
    }
}