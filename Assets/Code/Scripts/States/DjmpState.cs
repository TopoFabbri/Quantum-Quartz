using System.Collections;
using UnityEngine;

namespace Code.Scripts.States
{
    public class DjmpState<T> : JumpState<T>
    {
        public bool JumpAvailable { get; private set; }

        public DjmpState(T id, StateSettings.StateSettings stateSettings, MonoBehaviour mb, Rigidbody2D rb,
            Transform transform) : base(id, stateSettings, mb, rb, transform)
        {
            Reset();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            rb.velocity = new Vector2(0f, 0f);
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
            if (HasJumped)
                return;
            
            JumpAvailable = true;
        }
    }
}