using Code.Scripts.FSM;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Movement state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoveState<T> : BaseState<T>
    {
        protected MoveSettings moveSettings;
        
        protected readonly Rigidbody2D rb;
        private readonly Transform transform;

        public float Input { get; private set; }

        public MoveState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform) : base(id, stateSettings)
        {
            settings = stateSettings;
            moveSettings = settings as MoveSettings;
            
            this.rb = rb;
            this.transform = transform;
        }
        
        public void SetInput(float input)
        {
            Input = input;
        }
        
        public override void OnFixedUpdate()
        {
            rb.AddForce(Input * moveSettings.accel * Time.fixedDeltaTime * Vector2.right, ForceMode2D.Force);
            
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -moveSettings.maxSpeed, moveSettings.maxSpeed), rb.velocity.y);
        }
        
        /// <summary>
        /// Check if player is on ground
        /// </summary>
        /// <returns>True if on ground</returns>
        public bool IsGrounded()
        {
            return Physics2D.OverlapCircle((Vector2)transform.position + moveSettings.groundCheckOffset, moveSettings.groundCheckRadius, moveSettings.groundLayer);
        }
    }
}