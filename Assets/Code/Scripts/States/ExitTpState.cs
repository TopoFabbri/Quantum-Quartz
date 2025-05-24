using Code.Scripts.FSM;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.States
{
    public class ExitTpState<T> : BaseState<T>
    {
        public bool Ended { get; private set; }
        
        private readonly SharedContext sharedContext;
        
        public ExitTpState(T id, SharedContext sharedContext) : base(id)
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

        public void OnEnd()
        {
            Ended = true;
            sharedContext.Rigidbody.isKinematic = false;
        }
    }
}