using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class TpState<T> : BaseState<T>
    {        
        private readonly SharedContext sharedContext;
        private readonly TpSettings tpSettings;
        
        public TpState(T id, TpSettings tpSettings, SharedContext sharedContext) : base(id)
        {
            this.sharedContext = sharedContext;
            this.tpSettings = tpSettings;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            sharedContext.Rigidbody.velocity = Vector2.zero;
            sharedContext.Rigidbody.isKinematic = true;
            sharedContext.GearFX.position = sharedContext.Transform.position + (Vector3)tpSettings.gearOffset;
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.Rigidbody.isKinematic = false;
        }
    }
}