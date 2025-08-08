using System.Collections;
using Code.Scripts.Camera;
using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Double jump state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DjmpState<T> : JumpState<T>
    {
        protected readonly DjmpSettings djmpSettings;
        private readonly ParticleSystem djmpParticleSystem;
        private readonly ParticleSystem djmpParticleSystem2;
        
        public DjmpState(T id, DjmpSettings stateSettings, SharedContext sharedContext, ParticleSystem djmpParticleSystem, ParticleSystem djmpParticleSystem2) : base(id, stateSettings.jumpSettings, sharedContext)
        {
            this.djmpSettings = stateSettings;
            this.djmpParticleSystem = djmpParticleSystem;
            this.djmpParticleSystem2 = djmpParticleSystem2;
            sharedContext.djmpAvailable = true;
        }

        public override void OnEnter()
        {
            sharedContext.Rigidbody.velocity = sharedContext.Speed = Vector2.zero;
            base.OnEnter();
            sharedContext.PlayerSfx.Djmp();
            djmpParticleSystem.Play();
            djmpParticleSystem2.Play();

            sharedContext.djmpAvailable = false;
            
            sharedContext.CamController?.Shake(djmpSettings.shakeDur, djmpSettings.shakeMag);
        }
    }
}