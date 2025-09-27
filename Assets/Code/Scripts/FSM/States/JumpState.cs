using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.States.Settings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Jump up state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JumpState<T> : MoveState<T>, INoCoyoteTime, IUnsafe
    {
        protected readonly JumpSettings jumpSettings;

        public bool HasJumped { get; protected set; }

        private float lastVel = 0;

        public JumpState(T id, JumpSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.moveSettings, sharedContext, stateSettings.jumpCurve)
        {
            this.jumpSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sharedContext.PlayerSfx.Jump();
            sharedContext.SetFalling(false);

            sharedContext.SpeedY = verticalVelocityCurve.SampleVelocity(sharedContext.jumpFallTime);
            sharedContext.Rigidbody.velocity = sharedContext.Speed;
            lastVel = sharedContext.Speed.y;

            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.airFriction;
            
            SpawnDust();
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.groundFriction;
            HasJumped = false;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            bool lowSpeed = Mathf.Abs(sharedContext.Rigidbody.velocity.y) < sharedContext.GlobalSettings.neutralSpeed;
            bool intendedLowSpeed = Mathf.Abs(lastVel) < sharedContext.GlobalSettings.neutralSpeed;
            if (!HasJumped || (sharedContext.jumpFallTime < verticalVelocityCurve.Duration) && (!lowSpeed || intendedLowSpeed))
            {
                HasJumped = true;
                sharedContext.jumpFallTime += Time.fixedDeltaTime;
                float vel = verticalVelocityCurve.SampleVelocity(sharedContext.jumpFallTime);
                sharedContext.SpeedY = (vel + lastVel) * 0.5f;
                sharedContext.Rigidbody.velocity = sharedContext.Speed;
                lastVel = vel;
            }
            else
            {
                sharedContext.SetFalling(true);
            }
        }

        /// <summary>
        /// Make dust at jump position
        /// </summary>
        public virtual void SpawnDust()
        {
            Vector2 position = (Vector2)sharedContext.Transform.position + sharedContext.GlobalSettings.groundCheckOffset;
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, sharedContext.GlobalSettings.groundCheckDistance, sharedContext.SolidFilter.layerMask);
            
            if (!hit.collider || (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform")))
                return;
            
            Transform parent = hit.collider.transform;
            
            Object.Instantiate(jumpSettings.dust, hit.point, Quaternion.identity, parent);
        }
    }
}