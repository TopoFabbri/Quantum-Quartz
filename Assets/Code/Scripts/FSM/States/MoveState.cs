using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.States.Settings;
using Code.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public MoveState(T id, MoveSettings stateSettings, SharedContext sharedContext, VelocityCurve verticalVelocityCurve = null) : base(id)
        {
            this.moveSettings = stateSettings;
            this.sharedContext = sharedContext;
            this.verticalVelocityCurve = verticalVelocityCurve;
        }

        public override void OnExit()
        {
            base.OnExit();
            inputSpeed = 0;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            sharedContext.Speed = sharedContext.Rigidbody.velocity;

            FlipCheck();

            if (WallCheck())
            {
                ResetSpeed();
            }
            else if (sharedContext.Input != 0)
            {
                if (Mathf.Sign(sharedContext.Input) == Mathf.Sign(sharedContext.Speed.x))
                {
                    // If moving in the direction of current velocity, inherit speed
                    inputSpeed = sharedContext.Speed.x / moveSettings.maxSpeed;
                }
                float accel = sharedContext.Input * Time.fixedDeltaTime * moveSettings.accel;
                inputSpeed = Mathf.Clamp(inputSpeed + accel, -1, 1);
            }
            else
            {
                inputSpeed = Mathf.Lerp(inputSpeed, 0, Time.fixedDeltaTime * sharedContext.Rigidbody.sharedMaterial.friction);
            }

            sharedContext.Rigidbody.velocity = sharedContext.Speed = new Vector2(inputSpeed * moveSettings.maxSpeed, sharedContext.Rigidbody.velocity.y);
        }

        /// <summary>
        /// Check if the player is moving
        /// </summary>
        /// <returns>True if not moving</returns>
        public bool StoppedMoving()
        {
            if (Mathf.Abs(inputSpeed * Time.fixedDeltaTime) >= moveSettings.minSpeed || sharedContext.Input != 0f)
                return false;

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
        }
        
        private void FlipCheck()
        {
            if (sharedContext.Input > 0f && inputSpeed < 0f || sharedContext.Input < 0f && inputSpeed > 0f)
                ResetSpeed();
        }
    }
}