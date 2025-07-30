
using Code.Scripts.Camera;
using Code.Scripts.FSM;
using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Interfaces;
using Code.Scripts.StateSettings;
using System;
using System.Collections;
using UnityEngine;

namespace Code.Scripts.Player
{
    [Serializable]
    public class SharedContext
    {
        public Rigidbody2D Rigidbody { get; private set; }
        public Collider2D Collider { get; private set; }
        public Transform Transform { get; private set; }
        public MonoBehaviour MonoBehaviour { get; private set; }
        public PlayerSfx PlayerSfx { get; private set; }
        public GlobalSettings GlobalSettings { get; private set; }
        public CameraController CamController { get; private set; }
        public RectTransform GearFX { get; private set; }
        public bool Falling { get; private set; }
        public bool IsGrounded { get; private set; }
        public Type PreviousStateType => stateMachine.PreviousState?.GetType();
        public Type CurrentStateType => stateMachine.CurrentState?.GetType();

        private ContactFilter2D _solidFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Default", "SolidTiles")
        };
        public ContactFilter2D SolidFilter => _solidFilter;

        private float _input = 0;
        public float Input
        {
            get
            {
                return BlockMoveInput ? 0 : _input;
            }
            set
            {
                _input = value;
                OnCheckFlip?.Invoke();
            }
        }

        private bool _blockMoveInput = false;
        public bool BlockMoveInput
        {
            get
            {
                return _blockMoveInput;
            }
            set
            {
                _blockMoveInput = value;
                OnCheckFlip?.Invoke();
            }
        }

        private Vector2 _checkpointPos = Vector2.negativeInfinity;
        public Vector2 CheckpointPos
        {
            get
            {
                return _checkpointPos;
            }
            set
            {
                _checkpointPos = value;
                Stats.SaveCheckpoint(value);
            }
        }

        private Vector2 _speed = Vector2.zero;
        public Vector2 Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                if (Time.time > curSpeedTimestamp)
                {
                    previousSpeed = _speed;
                    curSpeedTimestamp = Time.time;
                }
                _speed = value;
            }
        }
        public float SpeedX { set { Speed = new Vector2(value, Speed.y); } }
        public float SpeedY { set { Speed = new Vector2(Speed.x, value); } }

        public ISpringable.SpringDefinition? spring = null;
        public bool facingRight = false;
        public bool died = false;
        public bool canCoyoteJump = false;
        public bool djmpAvailable = false;
        public bool inWallCooldown = false;
        public float jumpFallTime = 0;
        public Vector2 previousSpeed = Vector2.zero;
        public event Action OnCheckFlip;

        private readonly FiniteStateMachine<string> stateMachine;
        private double curSpeedTimestamp;
        private Coroutine wallCooldownCoroutine;

        public SharedContext(Rigidbody2D rb, Collider2D col, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx, GlobalSettings globalSettings, FiniteStateMachine<string> stateMachine, RectTransform gearFX)
        {
            Rigidbody = rb;
            Collider = col;
            Transform = transform;
            MonoBehaviour = mb;
            PlayerSfx = playerSfx;
            GlobalSettings = globalSettings;
            this.stateMachine = stateMachine;
            GearFX = gearFX;
            
            InputManager.Move += OnMoveHandler;

            if (UnityEngine.Camera.main?.transform.parent != null)
            {
                UnityEngine.Camera.main.transform.parent.TryGetComponent(out CameraController camController);
                CamController = camController;
            }
        }

        ~SharedContext()
        {
            InputManager.Move -= OnMoveHandler;
        }

        /// <summary>
        /// Handle player move action
        /// </summary>
        /// <param name="input">Input value</param>
        void OnMoveHandler(Vector2 input)
        {
            Input = input.x;
        }

        /// <summary>
        /// Check if player is on ground, and update IsGrounded
        /// </summary>
        /// <returns>True if on the ground</returns>
        public bool RecalculateIsGrounded()
        {
            bool grounded = false;

            float hitDist = float.NegativeInfinity;
            if (Mathf.Abs(Rigidbody.velocity.y) <= GlobalSettings.neutralSpeed || GlobalSettings.shouldDraw)
            {
                Vector2 startPos = (Vector2)Transform.position + GlobalSettings.groundCheckOffset;
                hitDist = HitFloor(Physics2D.RaycastAll(startPos, Vector2.down, GlobalSettings.groundCheckDistance, GlobalSettings.groundLayer));
                grounded = hitDist >= 0; // Uses '>=' to try and avoid the weird falling on platform glitch

                if (GlobalSettings.shouldDraw)
                {
                    Debug.DrawLine(startPos, startPos + Vector2.down * GlobalSettings.groundCheckDistance, grounded ? Color.green : Color.red);
                }

                if (!grounded || GlobalSettings.shouldDraw)
                {
                    float tempHitDist = GetEdge(true);
                    grounded |= tempHitDist > 0;
                    hitDist = Mathf.Max(tempHitDist, hitDist);

                    if (!grounded || GlobalSettings.shouldDraw)
                    {
                        tempHitDist = GetEdge(false);
                        grounded |= tempHitDist > 0;
                        hitDist = Mathf.Max(tempHitDist, hitDist);
                        grounded &= Mathf.Abs(Rigidbody.velocity.y) <= GlobalSettings.neutralSpeed; //If GlobalSettings.shouldDraw forced execution to reach this far, enforce velocity requirement
                    }
                }
            }

            if (grounded && hitDist < GlobalSettings.minGroundDist)
            {
                Transform.position += Vector3.up * (GlobalSettings.minGroundDist - hitDist);
            }

            IsGrounded = grounded;
            return IsGrounded;
        }

        private float GetEdge(bool right)
        {
            Vector2 startPos = (Vector2)Transform.position + GlobalSettings.groundCheckOffset + (right ? Vector2.right : Vector2.left) * GlobalSettings.edgeCheckDis;
            float edgeDist = HitFloor(Physics2D.RaycastAll(startPos, Vector2.down, GlobalSettings.edgeCheckLength, GlobalSettings.groundLayer));

            if (GlobalSettings.shouldDraw)
            {
                Debug.DrawLine(startPos, startPos + Vector2.down * GlobalSettings.edgeCheckLength, float.IsNegativeInfinity(edgeDist) ? Color.red : Color.green);
            }

            return edgeDist;
        }

        private float HitFloor(RaycastHit2D[] hits)
        {
            float backupDist = float.NegativeInfinity;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform")))
                {
                    if (hit.distance > 0)
                    {
                        return hit.distance;
                    }
                    else
                    {
                        backupDist = hit.distance;
                    }
                }
            }
            return backupDist;
        }

        public void SetFalling(bool falling)
        {
            Falling = falling;
            jumpFallTime = 0;
        }

        public void DoWallCooldown(float wallCooldown)
        {
            if (wallCooldownCoroutine != null)
            {
                MonoBehaviour.StopCoroutine(wallCooldownCoroutine);
            }
            wallCooldownCoroutine = MonoBehaviour.StartCoroutine(WallCooldown(wallCooldown));
        }

        private IEnumerator WallCooldown(float wallCooldown)
        {
            inWallCooldown = true;
            yield return new WaitForSeconds(wallCooldown);
            inWallCooldown = false;
        }
    }
}
