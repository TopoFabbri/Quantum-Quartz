using Code.Scripts.FSM;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class MoveState<T> : BaseState<T>
    {
        private MoveSettings MoveSettings => settings as MoveSettings;
        
        private readonly Rigidbody2D rb;

        public float Input { get; private set; }

        public MoveState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb) : base(id, stateSettings)
        {
            settings = stateSettings;
            
            this.rb = rb;
        }
        
        public void UpdateInput(float input)
        {
            Input = input;
        }
        
        public override void OnFixedUpdate()
        {
            rb.AddForce(Input * MoveSettings.accel * Time.fixedDeltaTime * Vector2.right, ForceMode2D.Force);
            
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -MoveSettings.maxSpeed, MoveSettings.maxSpeed), rb.velocity.y);
        }
    }
}