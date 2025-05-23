using System.Linq;
using Code.Scripts.FSM;
using Code.Scripts.StateSettings;
using UnityEngine;
using Code.Scripts.Player;

namespace Code.Scripts.States
{
    /// <summary>
    /// Movement state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoveState<T> : BaseState<T>
    {
        protected readonly MoveSettings moveSettings;

        protected readonly PlayerState.SharedContext sharedContext;

        private static float _speed;
        protected bool canMove = true;

        public float Speed => _speed;

        public MoveState(T id, MoveSettings stateSettings, PlayerState.SharedContext sharedContext) : base(id)
        {
            this.moveSettings = stateSettings;
            this.sharedContext = sharedContext;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            
            FlipCheck();

            if (canMove && sharedContext.Input != 0)
                _speed = Mathf.Clamp(_speed + sharedContext.Input * Time.fixedDeltaTime * moveSettings.accel, -moveSettings.maxSpeed, moveSettings.maxSpeed);
            else
                DecreaseSpeed();
            
            if (WallCheck())
                _speed = 0f;

            sharedContext.Transform.Translate(Vector2.right * (_speed * Time.fixedDeltaTime));
        }
        
        /// <summary>
        /// Apply ground friction
        /// </summary>
        public void DecreaseSpeed()
        {
            _speed = Mathf.Lerp(_speed, 0, Time.fixedDeltaTime * moveSettings.groundFriction);
        }

        /// <summary>
        /// Check if the player is moving
        /// </summary>
        /// <returns>True if not moving</returns>
        public bool StoppedMoving()
        {
            if (!(Mathf.Abs(_speed * Time.fixedDeltaTime) < moveSettings.minSpeed) || sharedContext.Input != 0f) return false;
            
            _speed = 0f;
            return true;
        }

        /// <summary>
        /// Check if the player is touching a wall
        /// </summary>
        public bool WallCheck()
        {
            Vector2 pos = (Vector2)sharedContext.Transform.position + Vector2.right * (moveSettings.wallCheckDis * sharedContext.Input);
            
            Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, moveSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (moveSettings.tags.Any(tag => collider.gameObject.CompareTag(tag)))
                    return true;

                if (!collider.gameObject.CompareTag("Platform")) continue;
                
                if (!collider.TryGetComponent(out PlatformEffector2D _))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Reset player speed
        /// </summary>
        public void ResetSpeed()
        {
            _speed = 0f;
        }
        
        private void FlipCheck()
        {
            if (sharedContext.Input > 0f && _speed < 0f || sharedContext.Input < 0f && _speed > 0f)
                _speed = 0f;
        }
    }
}