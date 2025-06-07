using Code.Scripts.Interfaces;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using Code.Scripts.Tools;
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
            base.OnFixedUpdate();

            if ((lastForce.y != 0 && sharedContext.Rigidbody.velocity.y == 0) || sharedContext.jumpFallTime >= springSettings.springCurve.Duration)
            {
                sharedContext.SetFalling(true);
            }
            else if (reachedOrigin)
            {
                sharedContext.jumpFallTime += Time.fixedDeltaTime;
                Vector2 force = CalculateSpring(spring.force, springSettings.springCurve, sharedContext.jumpFallTime, fallSettings.fallCurve, ref fallTime);

                sharedContext.speed = (force + lastForce) * 0.5f + Vector2.right * sharedContext.speed.x;
                sharedContext.Rigidbody.velocity = sharedContext.speed;
                lastForce = force;

                /*
                Vector2 force = springSettings.springCurve.SampleVelocity(sharedContext.jumpFallTime) * spring.force;

                // Radially interpolate the influence of the fall curve based on the spring force's direction
                // - Upwards spring: 0x
                // - Horizontal spring: 1x
                // - 45 degree spring: 0.785x
                // - Downward spring: Alternative approach
                //   + Uses the strongest downward force, fall or spring
                //   + When spring is stronger, fallTime is advanced to match the downwards speed
                if (spring.force.y >= 0)
                {
                    float fallInfluence = 1 - spring.force.normalized.y;
                    fallTime += Time.fixedDeltaTime * fallInfluence;
                    float fallForce = fallInfluence * fallSettings.fallCurve.SampleVelocity(fallTime);
                    force.y = force.y * (1 - fallInfluence) + fallForce;
                }
                else
                {
                    // Downwards spring
                    fallTime += Time.fixedDeltaTime;
                    float fallSpeed = fallSettings.fallCurve.SampleVelocity(fallTime);

                    if (force.y > fallSpeed) // -1 > -2
                    {
                        force.y = fallSpeed;
                    }
                    else
                    {
                        // If spring force is stronger than gravity, try and adjust fallTime to catch up
                        float newFallTime = fallTime;
                        float newFallSpeed = fallSpeed;
                        for (int i = 0; (Mathf.Abs(force.y - newFallSpeed) > 0.001f) && i < 3; i++)
                        {
                            float fallAcceleration = fallSettings.fallCurve.SampleAcceleration(newFallTime);
                            float diff = force.y - newFallSpeed;
                            float timeBoost = diff / fallAcceleration;
                            float s1 = fallSettings.fallCurve.SampleVelocity(newFallTime + timeBoost * 0.5f);
                            float s2 = fallSettings.fallCurve.SampleVelocity(newFallTime + timeBoost);
                            if (Mathf.Abs(force.y - s1) < Mathf.Abs(force.y - s2))
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

                        if (Mathf.Abs(force.y - newFallSpeed) < Mathf.Abs(force.y - fallSpeed))
                        {
                            fallTime = newFallTime;
                        }
                    }
                }

                sharedContext.speed = (force + lastForce) * 0.5f + Vector2.right * sharedContext.speed.x;
                sharedContext.Rigidbody.velocity = sharedContext.speed;
                lastForce = force;
                */
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
        }

        public static Vector2 CalculateSpring(Vector2 springForce, VelocityCurve springCurve, float springTime, VelocityCurve fallCurve, ref float fallTime)
        {
            Vector2 force = springCurve.SampleVelocity(springTime) * springForce;

            // Radially interpolate the influence of the fall curve based on the spring force's direction
            // - Upwards spring: 0x
            // - Horizontal spring: 1x
            // - 45 degree spring: 0.785x
            // - Downward spring: Alternative approach
            //   + Uses the strongest downward force, fall or spring
            //   + When spring is stronger, fallTime is advanced to match the downwards speed
            if (springForce.y >= 0)
            {
                float fallInfluence = 1 - springForce.normalized.y;
                fallTime += Time.fixedDeltaTime * fallInfluence;
                float fallForce = fallInfluence * fallCurve.SampleVelocity(fallTime);
                force.y = force.y * (1 - fallInfluence) + fallForce;
            }
            else
            {
                // Downwards spring
                fallTime += Time.fixedDeltaTime;
                float fallSpeed = fallCurve.SampleVelocity(fallTime);

                if (force.y > fallSpeed) // -1 > -2
                {
                    force.y = fallSpeed;
                }
                else
                {
                    // If spring force is stronger than gravity, try and adjust fallTime to catch up
                    float newFallTime = fallTime;
                    float newFallSpeed = fallSpeed;
                    for (int i = 0; (Mathf.Abs(force.y - newFallSpeed) > 0.001f) && i < 3; i++)
                    {
                        float fallAcceleration = fallCurve.SampleAcceleration(newFallTime);
                        float diff = force.y - newFallSpeed;
                        float timeBoost = diff / fallAcceleration;
                        float s1 = fallCurve.SampleVelocity(newFallTime + timeBoost * 0.5f);
                        float s2 = fallCurve.SampleVelocity(newFallTime + timeBoost);
                        if (Mathf.Abs(force.y - s1) < Mathf.Abs(force.y - s2))
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

                    if (Mathf.Abs(force.y - newFallSpeed) < Mathf.Abs(force.y - fallSpeed))
                    {
                        fallTime = newFallTime;
                    }
                }
            }

            return force;
        }
    }
}