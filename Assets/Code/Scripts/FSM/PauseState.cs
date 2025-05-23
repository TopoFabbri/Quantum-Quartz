using Code.Scripts.FSM;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.States
{
    public class PauseState<T> : BaseState<T>
    {
        private readonly PlayerState.SharedContext sharedContext;
        
        private Vector2 velocity;
        private float angularVelocity;
        private float drag;
        private float gravityScale;
        private bool isKinematic;

        public PauseState(T id, PlayerState.SharedContext sharedContext) : base(id)
        {
            this.sharedContext = sharedContext;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            velocity = sharedContext.Rigidbody.velocity;
            angularVelocity = sharedContext.Rigidbody.angularVelocity;
            drag = sharedContext.Rigidbody.drag;
            gravityScale = sharedContext.Rigidbody.gravityScale;
            isKinematic = sharedContext.Rigidbody.isKinematic;

            sharedContext.Rigidbody.velocity = Vector2.zero;
            sharedContext.Rigidbody.angularVelocity = 0f;
            sharedContext.Rigidbody.drag = 0f;
            sharedContext.Rigidbody.gravityScale = 0f;
            sharedContext.Rigidbody.isKinematic = true;
        }
        
        public override void OnExit()
        {
            base.OnExit();

            sharedContext.Rigidbody.isKinematic = isKinematic;
            sharedContext.Rigidbody.gravityScale = gravityScale;
            sharedContext.Rigidbody.drag = drag;
            sharedContext.Rigidbody.angularVelocity = angularVelocity;
            sharedContext.Rigidbody.velocity = velocity;
        }
    }
}