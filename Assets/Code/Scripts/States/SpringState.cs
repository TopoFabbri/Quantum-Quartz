using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class SpringState<T> : MoveState<T>
    {
        protected readonly SpringSettings springSettings;

        public bool IsActivated { get; private set; }

        public SpringState(T id, SpringSettings stateSettings, Rigidbody2D rb, Transform transform) : base(id, stateSettings.moveSettings, rb, transform)
        {
            this.springSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            IsActivated = false;
            rb.sharedMaterial.friction = moveSettings.airFriction;
        }

        public override void OnExit()
        {
            base.OnExit();
            rb.sharedMaterial.friction = moveSettings.groundFriction;
        }

        public void Activate()
        {
            IsActivated = true;
        }
    }
}