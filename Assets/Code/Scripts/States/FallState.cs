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

        public FallState(T id, FallSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.moveSettings, sharedContext, stateSettings.fallCurve)
        {
            this.fallSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            sharedContext.SetFalling(true);

            sharedContext.speed.y = verticalVelocityCurve.SampleVelocity(sharedContext.jumpFallTime);
            sharedContext.Rigidbody.velocity = sharedContext.speed;

            if (!typeof(INoCoyoteTime).IsAssignableFrom(sharedContext.PreviousStateType))
            {
                sharedContext.canCoyoteJump = true;
                sharedContext.MonoBehaviour.StartCoroutine(StopCoyoteTime());
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

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (sharedContext.Rigidbody.velocity.y != 0 || !sharedContext.IsGrounded)
            {
                sharedContext.jumpFallTime = sharedContext.jumpFallTime + Time.fixedDeltaTime;
                float vel = verticalVelocityCurve.SampleVelocity(sharedContext.jumpFallTime);
                sharedContext.speed.y = (vel + sharedContext.speed.y) * 0.5f;
                sharedContext.Rigidbody.velocity = sharedContext.speed;
                sharedContext.speed.y = vel;
            }
            else
            {
                sharedContext.SetFalling(false);
            }
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