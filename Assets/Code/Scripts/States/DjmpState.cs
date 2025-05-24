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

        public bool JumpAvailable { get; private set; }

        private readonly CameraController camController;
        
        public DjmpState(T id, DjmpSettings stateSettings, SharedContext sharedContext, ParticleSystem djmpParticleSystem, ParticleSystem djmpParticleSystem2) : base(id, stateSettings.jumpSettings, sharedContext)
        {
            this.djmpSettings = stateSettings;
            this.djmpParticleSystem = djmpParticleSystem;
            this.djmpParticleSystem2 = djmpParticleSystem2;

            UnityEngine.Camera.main?.transform.parent?.TryGetComponent(out camController);
                    
            Reset();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sharedContext.PlayerSfx.Djmp();

            djmpParticleSystem.Play();
            djmpParticleSystem2.Play();

            sharedContext.Rigidbody.velocity = Vector2.zero;
            
            if (camController)
                camController.Shake(djmpSettings.shakeDur, djmpSettings.shakeMag);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            HasJumped = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (!sharedContext.IsGrounded())
                HasJumped = true;
        }

        protected override IEnumerator JumpOnFU()
        {
            yield return new WaitForFixedUpdate();

            sharedContext.Rigidbody.AddForce(jumpSettings.jumpForce * Vector2.up, ForceMode2D.Impulse);
            JumpAvailable = false;
        }
        
        /// <summary>
        /// Set jump available
        /// </summary>
        public void Reset()
        {
            JumpAvailable = true;
        }
    }
}