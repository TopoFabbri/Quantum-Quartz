using System.Collections;
using Code.Scripts.Camera;
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
        private DjmpSettings DjmpSettings => settings as DjmpSettings;
        
        public bool JumpAvailable { get; private set; }

        private readonly CameraController camController;
        
        public DjmpState(T id, StateSettings.StateSettings stateSettings, MonoBehaviour mb, Rigidbody2D rb,
            Transform transform) : base(id, stateSettings, mb, rb, transform)
        {
            UnityEngine.Camera.main?.transform.parent?.TryGetComponent(out camController);
                    
            Reset();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            rb.velocity = Vector2.zero;
            
            if (camController)
                camController.Shake(DjmpSettings.shakeDur, DjmpSettings.shakeMag);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            HasJumped = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (!IsGrounded())
                HasJumped = true;
        }

        protected override IEnumerator JumpOnFU()
        {
            yield return new WaitForFixedUpdate();
            
            rb.AddForce(JumpSettings.jumpForce * Vector2.up, ForceMode2D.Impulse);
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