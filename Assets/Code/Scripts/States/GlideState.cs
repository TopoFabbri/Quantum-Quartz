using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class GlideState<T> : FallState<T>
    {
        GlideSettings Settings => settings as GlideSettings;

        private float prevGravScale;
        
        public GlideState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx) : base(id, stateSettings, rb, transform, mb, playerSfx)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            prevGravScale = rb.gravityScale;
            rb.gravityScale = 0f;
        }

        public override void OnExit()
        {
            base.OnExit();

            rb.gravityScale = prevGravScale;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            Vector2 vector2 = rb.velocity;
            vector2.y = -Settings.fallSpeed * Time.fixedDeltaTime;
            rb.velocity = vector2;
        }
    }
}