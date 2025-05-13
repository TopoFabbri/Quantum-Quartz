using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class SpringState<T> : MoveState<T>
    {
        protected SpringSettings SpringSettings => settings as SpringSettings;

        public bool IsActivated { get; private set; }

        public SpringState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform)
            : base(id, stateSettings, rb, transform) {}

        public override void OnEnter()
        {
            base.OnEnter();
            IsActivated = false;
            rb.sharedMaterial.friction = SpringSettings.airFriction;
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