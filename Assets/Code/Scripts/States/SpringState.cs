using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class SpringState<T> : MoveState<T>
    {
        protected readonly SpringSettings springSettings;

        public bool IsActivated { get; private set; }

        public SpringState(T id, SpringSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.moveSettings, sharedContext)
        {
            this.springSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            IsActivated = false;
            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.airFriction;
        }

        public override void OnExit()
        {
            base.OnExit();
            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.groundFriction;
        }

        public void Activate()
        {
            IsActivated = true;
        }
    }
}