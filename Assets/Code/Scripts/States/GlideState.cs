using Code.Scripts.Colors;
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
            
            barController.AddBar(ColorSwitcher.QColour.Yellow, Settings.regenSpeed, Settings.staminaMitigation, Settings.initStaminaCut);
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
            vector2.y = -Settings.fallSpeed * Time.fixedDeltaTime;
            rb.velocity = vector2;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            barController.GetBar(ColorSwitcher.QColour.Yellow).Use();
        }
    }
}