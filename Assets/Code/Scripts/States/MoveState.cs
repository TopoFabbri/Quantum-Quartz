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

        public float Input { get; protected set; }
        public float Speed => _speed;

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
                DecreaseSpeed();
            
            if (WallCheck())
                _speed = 0f;
            
            transform.Translate(Vector2.right * (_speed * Time.deltaTime));
        }
        
        /// <summary>
        /// Apply ground friction
        /// </summary>
        public void DecreaseSpeed()
        {
            _speed = Mathf.Lerp(_speed, 0, Time.deltaTime * moveSettings.groundFriction);
        }
        
        /// <summary>
        /// Check if player is on ground
        /// </summary>
        /// <returns>True if on ground</returns>
        public bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + moveSettings.groundCheckOffset, Vector2.down, moveSettings.groundCheckRadius, moveSettings.groundLayer);
            bool grounded = hit.collider && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform"));
            
            Debug.DrawLine((Vector2)transform.position + moveSettings.groundCheckOffset, (Vector2)transform.position + moveSettings.groundCheckOffset + Vector2.down * moveSettings.groundCheckRadius, grounded ? Color.green : Color.red);
            
            return grounded;
        }

        /// <summary>
        /// Check if player is moving
        /// </summary>
        /// <returns>True if not moving</returns>
        public bool StoppedMoving()
        {
            if (!(Mathf.Abs(_speed * Time.deltaTime) < moveSettings.minSpeed) || Input != 0f) return false;
            
            _speed = 0f;
            return true;
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