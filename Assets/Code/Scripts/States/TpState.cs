using Code.Scripts.FSM;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.States
{
    public class TpState<T> : BaseState<T>
    {
        public bool Ended { get; private set; }
        
        private readonly PlayerState.SharedContext sharedContext;
        
        public TpState(T id, PlayerState.SharedContext sharedContext) : base(id)
        {
            this.sharedContext = sharedContext;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            sharedContext.Rigidbody.velocity = Vector2.zero;
            sharedContext.Rigidbody.isKinematic = true;
            Ended = false;
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.Rigidbody.isKinematic = false;
        }
        
        public void OnEnd()
        {
            Ended = true;
        }
    }
}