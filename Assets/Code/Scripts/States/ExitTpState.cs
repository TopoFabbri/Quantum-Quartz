using Code.Scripts.FSM;
using UnityEngine;

namespace Code.Scripts.States
{
    public class ExitTpState<T> : BaseState<T>
    {
        public bool Ended { get; private set; }
        
        private readonly Rigidbody2D rb;
        
        public ExitTpState(T id, Rigidbody2D rb) : base(id)
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

        public void OnEnd()
        {
            Ended = true;
            rb.isKinematic = false;
        }
    }
}