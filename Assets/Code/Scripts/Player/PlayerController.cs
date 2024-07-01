using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Animation;
using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Input;
using Code.Scripts.Level;
using Code.Scripts.Platforms;
using Code.Scripts.States;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage player actions
    /// </summary>
    public class PlayerController : MonoBehaviour, IKillable
    {
        private FiniteStateMachine<string> fsm;

        // States
        private IdleState<string> idleState;
        private MoveState<string> moveState;
        private JumpState<string> jumpState;
        private FallState<string> fallState;
        private DashState<string> dashState;
        private DjmpState<string> djmpState;
        private DeathState<string> dethState;
        private SpawnState<string> spwnState;

        [SerializeField] private StateSettings.StateSettings[] stateSettings;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private TextMeshProUGUI stateTxt;
        [SerializeField] private List<GameObject> flipObjects = new();
        [SerializeField] private FsmAnimationController fsmAnimController;
        [SerializeField] private DeathController deathController;
        [SerializeField] private ParticleSystem dashPs;
        [SerializeField] private ParticleSystem djmpPs;
                
        [SerializeField] private SpriteRenderer sprite;

        private bool facingRight;
        private bool jumpPressed;
        private bool dashPressed;
        private bool djmpPressed;
        private bool falling;
        private bool died;

        private event Action<bool> OnFlip;

        private void Awake()
        {
            InitFsm();
        }

        private void Start()
        {
            FaceRight();
        }

        private void OnEnable()
        {
            jumpState.onEnter += OnEnterJumpHandler;
            fallState.onEnter += OnEnterFallHandler;
            dashState.onEnter += OnEnterDashHandler;
            djmpState.onEnter += OnEnterDjmpHandler;

            dethState.onExit += OnExitDeathHandler;
            spwnState.onExit += OnExitSpawnHandler;
            
            InputManager.Move += OnMoveHandler;
            InputManager.Jump += OnJumpPressedHandler;
            InputManager.Dash += OnDashHandler;
            InputManager.Djmp += OnDjmpHandler;

            ColorSwitcher.ColorChanged += OnChangedColorHandler;

            if (fsmAnimController)
            {
                fsm.StateChanged += fsmAnimController.OnStateChangedHandler;
                OnFlip += fsmAnimController.OnFlipHandler;
            }
        }

        private void OnDisable()
        {
            jumpState.onEnter -= OnEnterJumpHandler;
            fallState.onEnter -= OnEnterFallHandler;
            dashState.onEnter -= OnEnterDashHandler;
            djmpState.onEnter -= OnEnterDjmpHandler;

            dethState.onExit -= OnExitDeathHandler;
            spwnState.onExit -= OnExitSpawnHandler;
            
            InputManager.Move -= OnMoveHandler;
            InputManager.Jump -= OnJumpPressedHandler;
            InputManager.Dash -= OnDashHandler;
            InputManager.Djmp -= OnDjmpHandler;

            ColorSwitcher.ColorChanged -= OnChangedColorHandler;

            if (fsmAnimController)
            {
                fsm.StateChanged -= fsmAnimController.OnStateChangedHandler;
                OnFlip -= fsmAnimController.OnFlipHandler;
            }
            
            fallState.OnExit();
        }

        private void Update()
        {
            if (moveState.IsGrounded())
                djmpState.Reset();

            fsm.Update();

            if (facingRight && moveState.Input < 0f || !facingRight && moveState.Input > 0f)
                Flip();

            if (stateTxt)
                stateTxt.text = fsm.CurrentState.ID;
        }

        private void FixedUpdate()
        {
            fsm.FixedUpdate();

            if (falling && rb.velocity.y == 0f)
                falling = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!(other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Platform")))
                return;

            if (!moveState.IsGrounded())
                return;

            falling = false;
            // rb.velocity = new Vector2(rb.velocity.x, 0f);
            
            if (other.gameObject.TryGetComponent(out ObjMovement obj))
                obj.AddPlayer(transform);
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Platform"))
                transform.parent = null;
        }

        /// <summary>
        /// Initialize the finite state machine
        /// </summary>
        private void InitFsm()
        {
            idleState = new IdleState<string>("Idle");
            moveState = new MoveState<string>("Move", stateSettings[0], rb, transform);
            jumpState = new JumpState<string>("Jump", stateSettings[1], this, rb, transform);
            fallState = new FallState<string>("Fall", stateSettings[2], rb, transform, this);
            dashState = new DashState<string>("Dash", stateSettings[3], rb, this);
            djmpState = new DjmpState<string>("Djmp", stateSettings[4], this, rb, transform);
            dethState = new DeathState<string>("Death", stateSettings[5], transform, rb, this);
            spwnState = new SpawnState<string>("Spawn", stateSettings[6], transform, rb, this);

            fsm = new FiniteStateMachine<string>();

            fsm.AddState(idleState);
            fsm.AddState(moveState);
            fsm.AddState(jumpState);
            fsm.AddState(fallState);
            fsm.AddState(dashState);
            fsm.AddState(djmpState);
            fsm.AddState(dethState);
            fsm.AddState(spwnState);

            FsmTransitions();

            fsm.SetCurrentState(spwnState);

            fsm.Init();

            if (!fsmAnimController) return;

            fsmAnimController.AddState(idleState.ID, 0);
            fsmAnimController.AddState(moveState.ID, 1);
            fsmAnimController.AddState(jumpState.ID, 2);
            fsmAnimController.AddState(fallState.ID, 3);
            fsmAnimController.AddState(dashState.ID, 4);
            fsmAnimController.AddState(djmpState.ID, 5);
            fsmAnimController.AddState(dethState.ID, 6);
            fsmAnimController.AddState(spwnState.ID, 7);
        }

        /// <summary>
        /// Add fsm transitions
        /// </summary>
        private void FsmTransitions()
        {
            fsm.AddTransition(idleState, moveState, ShouldEnterMove);
            fsm.AddTransition(idleState, jumpState, () => jumpPressed && moveState.IsGrounded());
            fsm.AddTransition(idleState, fallState, () => rb.velocity.y < 0 && !moveState.IsGrounded());
            fsm.AddTransition(idleState, dashState, () => dashPressed);
            fsm.AddTransition(idleState, djmpState, () => djmpPressed);
            fsm.AddTransition(idleState, dethState, () => died);

            fsm.AddTransition(moveState, idleState, moveState.StoppedMoving);
            fsm.AddTransition(moveState, jumpState, () => jumpPressed && moveState.IsGrounded());
            fsm.AddTransition(moveState, fallState, () => rb.velocity.y < 0);
            fsm.AddTransition(moveState, dashState, () => dashPressed);
            fsm.AddTransition(moveState, djmpState, () => djmpPressed);
            fsm.AddTransition(moveState, dethState, () => died);

            fsm.AddTransition(jumpState, fallState, () => rb.velocity.y < 0);
            fsm.AddTransition(jumpState, idleState, () => moveState.IsGrounded() && jumpState.HasJumped && rb.velocity.y <= 0f);
            fsm.AddTransition(jumpState, dashState, () => dashPressed);
            fsm.AddTransition(jumpState, djmpState, () => djmpPressed);
            fsm.AddTransition(jumpState, dethState, () => died);

            fsm.AddTransition(fallState, moveState, () => !falling && ShouldEnterMove());
            fsm.AddTransition(fallState, idleState, () => !falling);
            fsm.AddTransition(fallState, dashState, () => dashPressed);
            fsm.AddTransition(fallState, djmpState, () => djmpPressed);
            fsm.AddTransition(fallState, jumpState, () => jumpPressed && fallState.CanCoyoteJump);
            fsm.AddTransition(fallState, dethState, () => died);

            fsm.AddTransition(dashState, fallState, () => dashState.Ended);
            fsm.AddTransition(dashState, dethState, () => died);

            fsm.AddTransition(djmpState, fallState, () => rb.velocity.y < 0f);
            fsm.AddTransition(djmpState, idleState, () => moveState.IsGrounded() && djmpState.HasJumped);
            fsm.AddTransition(djmpState, dethState, () => died);

            fsm.AddTransition(dethState, spwnState, () => dethState.Ended);
            
            fsm.AddTransition(spwnState, idleState, () => spwnState.Ended);
        }

        /// <summary>
        /// Force character to face right
        /// </summary>
        private void FaceRight()
        {
            if (!facingRight)
                Flip();
        }
        
        /// <summary>
        /// Flip character
        /// </summary>
        private void Flip()
        {
            if (fsm.CurrentState == dashState)
                return;

            facingRight = !facingRight;
            dashState.Flip();

            foreach (GameObject flipObject in flipObjects)
            {
                flipObject.transform.Rotate(0f, 180f, 0f);
                flipObject.transform.localPosition = new Vector3(-flipObject.transform.localPosition.x,
                    flipObject.transform.localPosition.y, flipObject.transform.localPosition.z);
            }
            
            
            OnFlip?.Invoke(facingRight);
        }

        /// <summary>
        /// Count and end jump buffer time
        /// </summary>
        /// <param name="jumpBufferTime">Time to count</param>
        /// <returns></returns>
        private IEnumerator EndJumpBufferTime(float jumpBufferTime)
        {
            yield return new WaitForSeconds(jumpBufferTime);
            jumpPressed = false;
        }

        /// <summary>
        /// Check if player should move
        /// </summary>
        /// <returns>True if player should move</returns>
        private bool ShouldEnterMove()
        {
            return moveState.Input != 0 && !moveState.WallCheck();
        }

        /// <summary>
        /// Manage player actions when color is changed
        /// </summary>
        /// <param name="color">New color</param>
        private void OnChangedColorHandler(ColorSwitcher.QColor color)
        {
            if (color != ColorSwitcher.QColor.Red)
                dashState.Reset();

            if (color != ColorSwitcher.QColor.Blue)
                djmpState.Reset();
        }

        /// <summary>
        /// Handle player move action
        /// </summary>
        /// <param name="input">Input value</param>
        private void OnMoveHandler(Vector2 input)
        {
            moveState.SetInput(input.x);
            jumpState.SetInput(input.x);
            fallState.SetInput(input.x);
            djmpState.SetInput(input.x);
        }

        /// <summary>
        /// Handle player jump action
        /// </summary>
        private void OnJumpPressedHandler()
        {
            jumpPressed = true;
            StartCoroutine(EndJumpBufferTime(jumpState.JumpBufferTime));
        }

        /// <summary>
        /// Handle player dash input
        /// </summary>
        /// <returns>True if player can dash</returns>
        private void OnDashHandler()
        {
            if (dashState.DashAvailable && ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Red)
                dashPressed = true;
        }

        /// <summary>
        /// Handle player MiniJump input
        /// </summary>
        private void OnDjmpHandler()
        {
            if (djmpState.JumpAvailable && ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Blue)
                djmpPressed = true;
        }

        /// <summary>
        /// Handle player HAS jumped
        /// </summary>
        private void OnEnterJumpHandler()
        {
            jumpPressed = false;

            if (fsm.PreviousState != fallState) return;
            
            Vector2 vector2 = rb.velocity;
            vector2.y = 0f;
            rb.velocity = vector2;
        }

        /// <summary>
        /// Handle player started falling
        /// </summary>
        private void OnEnterFallHandler()
        {
            falling = true;
            
            if (fsm.PreviousState != jumpState && fsm.PreviousState != djmpState)
                fallState.StartCoyoteTime();
        }

        /// <summary>
        /// Handle player started dashing
        /// </summary>
        private void OnEnterDashHandler()
        {
            dashPressed = false;
            
            dashPs.Play();
        }

        /// <summary>
        /// Handle player started Djump
        /// </summary>
        private void OnEnterDjmpHandler()
        {
            djmpPressed = false;
            
            djmpPs.Play();
        }
        
        /// <summary>
        /// Handle player exited death state
        /// </summary>
        private void OnExitDeathHandler()
        {
            deathController.Die();
            died = false;
        }
        
        /// <summary>
        /// Handle player exited spawn state
        /// </summary>
        private void OnExitSpawnHandler()
        {
            jumpPressed = false;
            dashPressed = false;
            djmpPressed = false;
            falling = false;
            died = false;
        }
        
        public void Kill()
        {
            if (died) return;
            
            died = true;
            
            dethState.Direction = new Vector2(-moveState.Input, -rb.velocity.normalized.y);
        }
    }
}