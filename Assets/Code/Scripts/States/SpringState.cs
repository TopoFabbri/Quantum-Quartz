using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class SpringState<T> : MoveState<T>
    {
        protected JumpSettings JumpSettings => settings as JumpSettings;

        bool active = false;
        public bool IsActivated => active;

        public SpringState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform)
            : base(id, stateSettings, rb, transform) {}

        public override void OnEnter()
        {
            base.OnEnter();
            active = false;
            rb.sharedMaterial.friction = JumpSettings.airFriction;
        }

        public override void OnExit()
        {
            base.OnExit();
            rb.sharedMaterial.friction = moveSettings.groundFriction;
        }

        public void Activate()
        {
            active = true;
        }
    }
}