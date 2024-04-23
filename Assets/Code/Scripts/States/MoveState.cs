using Code.Scripts.FSM;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class MoveState<T> : BaseState<T>
    {
        protected MoveSettings moveSettings;
        
        protected readonly Rigidbody2D rb;

        public float Input { get; private set; }

        public MoveState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb) : base(id, stateSettings)
        {
            settings = stateSettings;
            moveSettings = settings as MoveSettings;
            
            this.rb = rb;
        }
        
        public void UpdateInput(float input)
        {
            Input = input;
        }
        
        public override void OnFixedUpdate()
        {
            rb.AddForce(Input * moveSettings.accel * Time.fixedDeltaTime * Vector2.right, ForceMode2D.Force);
            
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -moveSettings.maxSpeed, moveSettings.maxSpeed), rb.velocity.y);
        }
    }
}