using UnityEngine;

namespace Code.Scripts.FSM
{
    public class PauseState<T> : BaseState<T>
    {
        private readonly Rigidbody2D rb;
        
        private Vector2 velocity;
        private float angularVelocity;
        private float drag;
        private float gravityScale;
        private bool isKinematic;

        public PauseState(T id, Rigidbody2D rb) : base(id)
        {
            this.rb = rb;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            velocity = rb.velocity;
            angularVelocity = rb.angularVelocity;
            drag = rb.drag;
            gravityScale = rb.gravityScale;
            isKinematic = rb.isKinematic;
            
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.drag = 0f;
            rb.gravityScale = 0f;
            rb.isKinematic = true;
        }
        
        public override void OnExit()
        {
            base.OnExit();
            
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
            rb.drag = drag;
            rb.gravityScale = gravityScale;
            rb.isKinematic = isKinematic;
        }
    }
}