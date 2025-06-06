using Code.Scripts.Interfaces;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class SpringState<T> : MoveState<T>, INoCoyoteTime
    {
        protected readonly SpringSettings springSettings;
        protected readonly FallSettings fallSettings;

        private ISpringable.SpringDefinition spring;
        private Vector2 lastForce = Vector2.zero;
        private float fallTime = 0; 

        private bool _reachedOrigin = false;
        private bool reachedOrigin {
            get
            {
                _reachedOrigin =  _reachedOrigin || Vector2.Dot(spring.origin - (Vector2)sharedContext.Transform.position, spring.force) < 0;
                return _reachedOrigin;
            }
            set
            {
                _reachedOrigin = value;
            }
        }

        public SpringState(T id, SpringSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.moveSettings, sharedContext)
        {
            this.springSettings = stateSettings;
            this.fallSettings = stateSettings.fallSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sharedContext.SetFalling(false);
            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.airFriction;

            spring = sharedContext.spring.Value;
            sharedContext.spring = null;
            sharedContext.speed = Vector2.zero;
            lastForce = sharedContext.speed;
            fallTime = 0;
            reachedOrigin = false;
        }

        public override void OnExit()
        {
            base.OnExit();
            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.groundFriction;
            sharedContext.jumpFallTime = fallTime;
        }

        public override void OnFixedUpdate()
        {
            if (sharedContext.jumpFallTime >= springSettings.springCurve.Duration)
            {
                sharedContext.SetFalling(true);
            }
            else if (reachedOrigin)
            {
                sharedContext.jumpFallTime += Time.fixedDeltaTime;
                Vector2 force = springSettings.springCurve.SampleVelocity(sharedContext.jumpFallTime) * spring.force;

                // Radially interpolate the influence of the fall curve based on the spring force's direction
                // - Upwards spring: 0x
                // - Horizontal spring: 1x
                // - 45 degree spring: 0.785x
                // - Downward spring: Not considered!!!
                //   + Should probably advance fallTime to match if spring force has greater velocity, and take fall force if greater
                float fallInfluence = 1 - spring.force.normalized.y;
                fallTime += Time.fixedDeltaTime * fallInfluence;
                float fallForce = fallInfluence * fallSettings.fallCurve.SampleVelocity(fallTime);
                force.y = force.y * (1 - fallInfluence) + fallForce;

                sharedContext.speed = (force + lastForce) * 0.5f;
                sharedContext.Rigidbody.velocity = sharedContext.speed;
                sharedContext.speed = force;
                lastForce = sharedContext.speed;
            }
            else
            {
                // Ensures the player reaches the spring's origin point before applying the impulse
                float distancePerStep = spring.force.magnitude * Time.fixedDeltaTime;
                Vector2 originOffset = spring.origin - (Vector2)sharedContext.Transform.position;
                float originDist = Vector2.Dot(originOffset, spring.force.normalized);
                float stepCount = originDist / distancePerStep;

                float stepMultiplier = 1 + stepCount % 1;
                if (stepMultiplier > 1.5f)
                {
                    stepMultiplier = stepCount % 1;
                }

                sharedContext.Rigidbody.velocity = spring.force * stepMultiplier;
            }

            // Apply movement only after the spring speed is calculated
            base.OnFixedUpdate();
        }
    }
}