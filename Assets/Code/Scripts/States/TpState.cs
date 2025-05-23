using Code.Scripts.FSM;
using UnityEngine;

namespace Code.Scripts.States
{
    public class TpState<T> : BaseState<T>
    {
        public bool Ended { get; private set; }
        
        private readonly Rigidbody2D rb;
        
        public TpState(T id, Rigidbody2D rb) : base(id)
        {
            this.rb = rb;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            Ended = false;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            rb.isKinematic = false;
        }
        
        public void OnEnd()
        {
            Ended = true;
        }
    }
}