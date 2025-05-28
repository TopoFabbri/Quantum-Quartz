using System.Collections;
using Code.Scripts.FSM;
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

        public FallState(T id, FallSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.moveSettings, sharedContext)
        {
            this.fallSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            sharedContext.falling = true;

            System.Type prevType = sharedContext.PreviousStateType;
            if (
                prevType != typeof(JumpState<string>) && prevType != typeof(DjmpState<string>)
                && prevType != typeof(DashState<string>) && prevType != typeof(WallJumpState<string>)
                && prevType != typeof(WallState<string>)
            )
            {
                StartCoyoteTime();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (sharedContext.IsGrounded)
            {
                sharedContext.PlayerSfx.Land();
                SpawnDust();
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            sharedContext.Rigidbody.velocity = new Vector2(0f, sharedContext.Rigidbody.velocity.y);
            
            if (-sharedContext.Rigidbody.velocity.y > fallSettings.maxFallSpeed)
                sharedContext.Rigidbody.velocity = new Vector2(sharedContext.Rigidbody.velocity.x, -fallSettings.maxFallSpeed);
        }

        public void StartCoyoteTime()
        {
            sharedContext.canCoyoteJump = true;
            sharedContext.MonoBehaviour.StartCoroutine(StopCoyoteTime());
        }

        private IEnumerator StopCoyoteTime()
        {
            yield return new WaitForSeconds(fallSettings.coyoteTime);
            sharedContext.canCoyoteJump = false;
        }

        private void SpawnDust()
        {
            Vector2 position = (Vector2)sharedContext.Transform.position + sharedContext.GlobalSettings.groundCheckOffset;

            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, sharedContext.GlobalSettings.groundCheckRadius, LayerMask.GetMask("Default"));

            if (hit.collider == null)
                return;

            if (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform"))
                return;

            Transform parent = hit.collider.transform;

            Object.Instantiate(fallSettings.dust, hit.point, Quaternion.identity, parent);
        }
    }
}