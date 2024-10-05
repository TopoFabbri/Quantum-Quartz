using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class GlideState<T> : FallState<T>
    {
        protected GlideSettings Settings => settings as GlideSettings;

        private float prevGravScale;
        private readonly BarController barController;
        
        public GlideState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx, BarController barController) : base(id, stateSettings, rb, transform, mb, playerSfx)
        {
            this.barController = barController;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            barController.SetVisibility(true);
            
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

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            barController.FillValue -= Settings.staminaMitigation * Time.deltaTime;
        }
    }
}