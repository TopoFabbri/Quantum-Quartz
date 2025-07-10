using System.Collections;
using System.Collections.Generic;
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
    public class FallState<T> : MoveState<T>, IUnsafe
    {
        protected readonly FallSettings fallSettings;

        private float lastVel = 0;
        private static ContactFilter2D contactFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Default")
        };

        public FallState(T id, FallSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.moveSettings, sharedContext, stateSettings.fallCurve)
        {
            this.fallSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (this.GetType() != typeof(FallState<T>) || !sharedContext.Falling)
            {
                sharedContext.SetFalling(true);
            }
            else if (sharedContext.Rigidbody.velocity.y < 0)
            {
                // If already falling, try and adjust jumpFallTime to catch up
                float ySpeed = sharedContext.Rigidbody.velocity.y;
                float fallSpeed = verticalVelocityCurve.SampleVelocity(sharedContext.jumpFallTime);
                float newFallTime = sharedContext.jumpFallTime;
                float newFallSpeed = fallSpeed;
                for (int i = 0; (Mathf.Abs(ySpeed - newFallSpeed) > sharedContext.GlobalSettings.neutralSpeed) && i < 3; i++)
                {
                    float fallAcceleration = fallSettings.fallCurve.SampleAcceleration(newFallTime);
                    float diff = ySpeed - newFallSpeed;
                    float timeBoost = Mathf.Max(diff / fallAcceleration, -newFallTime);
                    float s1 = fallSettings.fallCurve.SampleVelocity(newFallTime + timeBoost * 0.5f);
                    float s2 = fallSettings.fallCurve.SampleVelocity(newFallTime + timeBoost);
                    if (Mathf.Abs(ySpeed - s1) < Mathf.Abs(ySpeed - s2))
                    {
                        newFallTime += timeBoost * 0.5f;
                        newFallSpeed = s1;
                    }
                    else
                    {
                        newFallTime += timeBoost;
                        newFallSpeed = s2;
                    }
                }

                if (Mathf.Abs(ySpeed - newFallSpeed) < Mathf.Abs(ySpeed - fallSpeed))
                {
                    sharedContext.jumpFallTime = newFallTime;
                }
            }

            sharedContext.SpeedY = verticalVelocityCurve.SampleVelocity(sharedContext.jumpFallTime);
            sharedContext.Rigidbody.velocity = sharedContext.Speed;
            lastVel = sharedContext.Speed.y;

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
                sharedContext.SetFalling(false);
                sharedContext.PlayerSfx.Land();
                SpawnDust();
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (Mathf.Abs(sharedContext.Rigidbody.velocity.y) > sharedContext.GlobalSettings.neutralSpeed || !sharedContext.IsGrounded)
            {
                sharedContext.jumpFallTime += Time.fixedDeltaTime;
                float vel = verticalVelocityCurve.SampleVelocity(sharedContext.jumpFallTime);
                sharedContext.SpeedY = (vel + lastVel) * 0.5f;
                sharedContext.Rigidbody.velocity = sharedContext.Speed;
                lastVel = vel;
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
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, sharedContext.GlobalSettings.groundCheckDistance, LayerMask.GetMask("Default"));

            if (hit.collider == null || (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform")))
                return;

            Transform parent = hit.collider.transform;

            Object.Instantiate(fallSettings.dust, hit.point, Quaternion.identity, parent);
        }
    }
}