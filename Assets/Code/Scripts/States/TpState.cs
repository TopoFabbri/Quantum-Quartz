using Code.Scripts.FSM;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.States
{
    public class TpState<T> : BaseState<T>
    {        
        private readonly SharedContext sharedContext;
        
        public TpState(T id, SharedContext sharedContext) : base(id)
        {
            this.sharedContext = sharedContext;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            sharedContext.Rigidbody.velocity = Vector2.zero;
            sharedContext.Rigidbody.isKinematic = true;
            sharedContext.GearFX.position = sharedContext.Transform.position + Vector3.up;
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.Rigidbody.isKinematic = false;
        }
    }
}