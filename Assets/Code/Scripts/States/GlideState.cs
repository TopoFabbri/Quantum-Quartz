using Code.Scripts.Colors;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class GlideState<T> : FallState<T>
    {
        protected readonly GlideSettings glideSettings;

        private float prevGravScale;
        private readonly BarController barController;
        
        public GlideState(T id, GlideSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx, BarController barController) : base(id, stateSettings.fallSettings, rb, transform, mb, playerSfx)
        {
            this.glideSettings = stateSettings;
            this.barController = barController;
            
            barController.AddBar(ColorSwitcher.QColour.Yellow, glideSettings.regenSpeed, glideSettings.staminaMitigation, glideSettings.initStaminaCut);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            prevGravScale = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            
            barController.GetBar(ColorSwitcher.QColour.Yellow).FirstUseCut();
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
            vector2.y = -glideSettings.fallSpeed * Time.fixedDeltaTime;
            rb.velocity = vector2;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            barController.GetBar(ColorSwitcher.QColour.Yellow).Use();
        }
    }
}