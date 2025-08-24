using System.Linq;
using Code.Scripts.FSM;
using Code.Scripts.StateSettings;
using UnityEngine;
using Code.Scripts.Player;
using Code.Scripts.Tools;
using System.Collections.Generic;
using System;

namespace Code.Scripts.States
{
    /// <summary>
    /// Movement state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoveState<T> : BaseState<T>
    {
        protected readonly MoveSettings moveSettings;

        protected readonly SharedContext sharedContext;
        protected readonly VelocityCurve verticalVelocityCurve;

        private float inputSpeed = 0;

        protected float Acceleration => moveSettings.accel * sharedContext.CurMovementModifier.accel;
        protected float MaxSpeed => moveSettings.maxSpeed * sharedContext.CurMovementModifier.maxSpeed;
        protected float MinSpeed => moveSettings.minSpeed * sharedContext.CurMovementModifier.minSpeed;
        protected float Friction => sharedContext.Rigidbody.sharedMaterial.friction * (sharedContext.IsGrounded ? sharedContext.CurMovementModifier.groundFriction : sharedContext.CurMovementModifier.airFriction);

        public MoveState(T id, MoveSettings stateSettings, SharedContext sharedContext, VelocityCurve verticalVelocityCurve = null) : base(id)
        {
            this.moveSettings = stateSettings;
            this.sharedContext = sharedContext;
            this.verticalVelocityCurve = verticalVelocityCurve;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.groundFriction;
            inputSpeed = sharedContext.Speed.x;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            sharedContext.SpeedY = sharedContext.Rigidbody.velocity.y;
            sharedContext.SpeedX = Math.Sign(sharedContext.Rigidbody.velocity.x) != Math.Sign(sharedContext.Speed.x) ? sharedContext.Rigidbody.velocity.x : sharedContext.Speed.x;

            //FlipCheck();

            if (WallCheck())
            {
                ResetSpeed();
            }
            else if (sharedContext.Input != 0)
            {
                if (Math.Sign(sharedContext.Input) == Math.Sign(sharedContext.Speed.x))
                {
                    // If moving in the direction of current velocity, inherit speed
                    inputSpeed = sharedContext.Speed.x;
                }
                float maxSpeed = MaxSpeed;
                float accel = sharedContext.Input * Time.fixedDeltaTime * Acceleration;
                inputSpeed += Mathf.Clamp(accel, Mathf.Min(-maxSpeed - inputSpeed, 0), Mathf.Max(maxSpeed - inputSpeed, 0));

                if (Mathf.Abs(inputSpeed) > maxSpeed || Math.Sign(sharedContext.Input) * -1 == Math.Sign(sharedContext.Speed.x))
                {
                    inputSpeed = Math.Sign(inputSpeed) * Mathf.Max(Mathf.Abs(inputSpeed) - Time.fixedDeltaTime * Friction, 0);
                }
            }
            else
            {
                inputSpeed = Math.Sign(inputSpeed) * Mathf.Max(Mathf.Abs(inputSpeed) - Time.fixedDeltaTime * Friction, 0);
            }

            sharedContext.Rigidbody.velocity = sharedContext.Speed = new Vector2(inputSpeed, sharedContext.Rigidbody.velocity.y);
        }

        /// <summary>
        /// Check if the player is moving
        /// </summary>
        /// <returns>True if not moving</returns>
        public bool StoppedMoving()
        {
            if (Mathf.Abs(sharedContext.Speed.x) >= MinSpeed)
                return false;

            if (sharedContext.Input != 0f)
            {
                if (Math.Sign(sharedContext.Input) != Math.Sign(sharedContext.Speed.x))
                {
                    ResetSpeed();
                }
                return false;
            }

            ResetSpeed();
            return true;
        }

        /// <summary>
        /// Check if the player is touching a wall
        /// </summary>
        public bool WallCheck()
        {
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            sharedContext.Collider.Cast(Vector2.right, sharedContext.SolidFilter, hits, sharedContext.GlobalSettings.wallCheckDis * Math.Sign(sharedContext.Input), true);

            foreach (RaycastHit2D hit in hits)
            {
                if (sharedContext.GlobalSettings.wallTags.Any(tag => hit.transform.CompareTag(tag)))
                    return true;

                if (!hit.transform.CompareTag("Platform"))
                    continue;
                
                if (!hit.transform.TryGetComponent(out PlatformEffector2D _))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Reset player speed
        /// </summary>
        public void ResetSpeed()
        {
            inputSpeed = 0f;
            sharedContext.Rigidbody.velocity = sharedContext.Speed = new Vector2(0, sharedContext.Rigidbody.velocity.y);
            sharedContext.AdvanceMovementModifierTransition(sharedContext.CurMovementModifier.stopStep);
        }
        
        private void FlipCheck()
        {
            if (sharedContext.Input > 0f && inputSpeed < 0f || sharedContext.Input < 0f && inputSpeed > 0f)
                ResetSpeed();
        }
    }
}