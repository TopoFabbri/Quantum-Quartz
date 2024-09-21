using System.Collections;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Jump up state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JumpState<T> : MoveState<T>
    {
        protected JumpSettings JumpSettings => settings as JumpSettings;

        protected readonly MonoBehaviour mb;

        public bool HasJumped { get; protected set; }
        public float JumpForce => JumpSettings.jumpForce;
        public float JumpBufferTime => JumpSettings.bufferTime;
        
        public JumpState(T id, StateSettings.StateSettings stateSettings, MonoBehaviour mb, Rigidbody2D rb,
            Transform transform) : base(id, stateSettings, rb, transform)
        {
            this.mb = mb;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            mb.StartCoroutine(JumpOnFU());

            rb.sharedMaterial.friction = JumpSettings.airFriction;
            
            SpawnDust();
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

        /// <summary>
        /// Wait for fixed update and jump
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator JumpOnFU()
        {
            yield return new WaitForFixedUpdate();

            rb.AddForce(JumpSettings.jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Make dust at jump position
        /// </summary>
        public virtual void SpawnDust()
        {
            Vector2 position = (Vector2)transform.position + JumpSettings.groundCheckOffset;
            
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, JumpSettings.groundCheckRadius, LayerMask.GetMask("Default"));
            
            if (hit.collider == null)
                return;
            
            if (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform"))
                return;
            
            Transform parent = hit.collider.transform;
            
            Object.Instantiate(JumpSettings.dust, hit.point, Quaternion.identity, parent);
        }
    }
}