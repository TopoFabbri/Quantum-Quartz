using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Falling state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FallState<T> : MoveState<T>
    {
        private FallSettings FallSettings => settings as FallSettings;
        
        public FallState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform) : base(id, stateSettings, rb, transform)
        {
            settings = stateSettings;
            moveSettings = FallSettings.moveSettings;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            rb.sharedMaterial.friction = FallSettings.moveSettings.airFriction;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            rb.sharedMaterial.friction = FallSettings.moveSettings.groundFriction;
        }
    }
}