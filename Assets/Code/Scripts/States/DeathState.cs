using System.Collections;
using Code.Scripts.Camera;
using Code.Scripts.FSM;
using Code.Scripts.Game;
using Code.Scripts.Level;
using Code.Scripts.Platforms;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Die state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeathState<T> : BaseState<T>, IDeathImmune, IUnsafe
    {
        protected readonly DeathSettings deathSettings;

        private readonly SharedContext sharedContext;

        public Vector2 Direction { get; set; }
        public bool Ended { get; private set; }

        private bool moving;
        private float speed;

        public DeathState(T id, DeathSettings stateSettings, SharedContext sharedContext) : base(id)
        {
            this.deathSettings = stateSettings;
            this.sharedContext = sharedContext;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sharedContext.PlayerSfx.Death();
            sharedContext.Collider.enabled = false;

            Direction = new Vector2(-sharedContext.Input, -sharedContext.Rigidbody.velocity.normalized.y);

            Ended = false;
            moving = true;
            sharedContext.Rigidbody.velocity = Vector2.zero;
            sharedContext.Rigidbody.isKinematic = true;

            sharedContext.CamController?.Shake(deathSettings.shakeDur, deathSettings.shakeMag);

            sharedContext.MonoBehaviour.StartCoroutine(WaitAndEnd());

            if (sharedContext.Transform.parent && sharedContext.Transform.parent.TryGetComponent(out ObjectMovement objMovement)) {
                objMovement.RemovePlayer(sharedContext.Transform);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            sharedContext.Transform.position = sharedContext.CheckpointPos;
            sharedContext.died = false;

            Stats.AddDeath();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            speed = moving
                ? speed + deathSettings.accel * Time.deltaTime
                : speed - deathSettings.accel * Time.deltaTime;

            speed = Mathf.Clamp(speed, 0f, deathSettings.maxSpeed);

            Vector2 target = (Vector2)sharedContext.Transform.position + Direction;
            sharedContext.Transform.position = Vector2.MoveTowards(sharedContext.Transform.position, target, speed * Time.deltaTime);
        }

        /// <summary>
        /// Wait for duration and set to ended
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitAndEnd()
        {
            yield return new WaitForSeconds(deathSettings.movingDuration);
            moving = false;

            yield return new WaitForSeconds(deathSettings.duration - deathSettings.movingDuration);
            Ended = true;
        }
    }
}