using System.Collections;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Falling state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FallState<T> : MoveState<T>
    {
        protected readonly FallSettings fallSettings;

        protected readonly MonoBehaviour mb;
        private readonly PlayerSfx playerSfx;
        public bool CanCoyoteJump { get; private set; }

        public FallState(T id, FallSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx) : base(id, stateSettings.moveSettings, rb, transform)
        {
            this.fallSettings = stateSettings;
            this.mb = mb;
            this.playerSfx = playerSfx;
        }

        public override void OnExit()
        {
            base.OnExit();

            if (IsGrounded())
                playerSfx.Land();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            rb.velocity = new Vector2(0f, rb.velocity.y);
            
            if (Mathf.Abs(rb.velocity.y) > fallSettings.maxFallSpeed)
                rb.velocity = new Vector2(rb.velocity.x, -fallSettings.maxFallSpeed);
        }

        public void StartCoyoteTime()
        {
            CanCoyoteJump = true;
            mb.StartCoroutine(StopCoyoteTime());
        }

        private IEnumerator StopCoyoteTime()
        {
            yield return new WaitForSeconds(fallSettings.coyoteTime);
            CanCoyoteJump = false;
        }
    }
}