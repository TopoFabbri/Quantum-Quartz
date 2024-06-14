using System.Collections.Generic;
using Code.Scripts.FSM;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Movement state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoveState<T> : BaseState<T>
    {
        protected MoveSettings moveSettings;
        
        protected readonly Rigidbody2D rb;
        protected readonly Transform transform;
        
        private static float _speed;

        public float Input { get; private set; }

        public MoveState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform) : base(id, stateSettings)
        {
            settings = stateSettings;
            moveSettings = settings as MoveSettings;
            
            this.rb = rb;
            this.transform = transform;
        }
        
        public void SetInput(float input)
        {
            Input = input;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (Input != 0)
                _speed = Mathf.Clamp(_speed + Input * Time.deltaTime * moveSettings.accel, -moveSettings.maxSpeed, moveSettings.maxSpeed);
            else
                _speed = Mathf.Lerp(_speed, 0, Time.deltaTime * moveSettings.groundFriction);

            if (_speed is <= 0.1f and >= -0.1f)
                _speed = 0f;
            
            if (WallCheck())
                _speed = 0f;
            
            transform.Translate(Vector2.right * (_speed * Time.deltaTime));
        }
        
        /// <summary>
        /// Check if player is on ground
        /// </summary>
        /// <returns>True if on ground</returns>
        public bool IsGrounded()
        {
            Debug.DrawLine((Vector2)transform.position + moveSettings.groundCheckOffset - Vector2.right * moveSettings.groundCheckRadius, (Vector2)transform.position + moveSettings.groundCheckOffset + Vector2.right * moveSettings.groundCheckRadius, Color.red);
            Debug.DrawLine((Vector2)transform.position + moveSettings.groundCheckOffset - Vector2.up * moveSettings.groundCheckRadius, (Vector2)transform.position + moveSettings.groundCheckOffset + Vector2.up * moveSettings.groundCheckRadius, Color.red);
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + moveSettings.groundCheckOffset, moveSettings.groundCheckRadius, moveSettings.groundLayer);
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Floor") || collider.gameObject.CompareTag("Platform"))
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Check if player is moving
        /// </summary>
        /// <returns>True if not moving</returns>
        public bool StoppedMoving()
        {
            return Mathf.Abs(_speed) < 3f && Input == 0f;
        }

        /// <summary>
        /// Check if player is touching a wall
        /// </summary>
        public bool WallCheck()
        {
            Vector2 pos = (Vector2)transform.position + Vector2.right * (moveSettings.wallCheckDis * Input);
            
            Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, moveSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Floor"))
                    return true;

                if (!collider.gameObject.CompareTag("Platform")) continue;
                
                if (!collider.TryGetComponent(out PlatformEffector2D platformEffector2D))
                    return true;
            }
            
            return false;
        }
    }
}