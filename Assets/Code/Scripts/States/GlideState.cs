using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class GlideState<T> : FallState<T>
    {
        protected readonly GlideSettings glideSettings;

        private readonly BarController barController;
        private readonly ParticleSystem glideParticleSystem;

        public GlideState(T id, GlideSettings stateSettings, SharedContext sharedContext, BarController barController, ParticleSystem glideParticleSystem) : base(id, stateSettings.fallSettings, sharedContext)
        {
            this.glideSettings = stateSettings;
            this.barController = barController;
            this.glideParticleSystem = glideParticleSystem;

            barController.AddBar(ColorSwitcher.QColour.Yellow, glideSettings.regenSpeed, glideSettings.staminaMitigation, glideSettings.initStaminaCut);
            barController.GetBar(ColorSwitcher.QColour.Yellow).AddConditionalRegenSpeed(() => !sharedContext.IsGrounded ? 0 : null);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sharedContext.PlayerSfx.PlayGlide();
            glideParticleSystem.Play();
            
            barController.GetBar(ColorSwitcher.QColour.Yellow).FirstUseCut();
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.PlayerSfx.StopGlide();
            glideParticleSystem.Stop();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            barController.GetBar(ColorSwitcher.QColour.Yellow).Use();
        }
    }
}