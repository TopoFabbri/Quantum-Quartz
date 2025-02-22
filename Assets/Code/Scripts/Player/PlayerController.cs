using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Animation;
using Code.Scripts.Camera;
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
        private TpState<string> tlptState;
        private ExitTpState<string> extpState;
        private WallState<string> wallState;
        private WallJumpState<string> wallJumpState;
        private GlideState<string> gldeState;
        private GrabState<string> grabState;
        private PauseState<string> pausState;
        private EdgeState<string> edgeState;

        [SerializeField] private StateSettings.StateSettings[] stateSettings;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private TextMeshProUGUI stateTxt;
        [SerializeField] private List<GameObject> flipObjects = new();
        [SerializeField] private FsmAnimationController fsmAnimController;
        [SerializeField] private DeathController deathController;
        [SerializeField] private ParticleSystem dashPs;
        [SerializeField] private ParticleSystem djmpPs;
        [SerializeField] private ParticleSystem djmpPs2;
        [SerializeField] private ParticleSystem gldePs;
        [SerializeField] private ParticleSystem wjmpPs;
        [SerializeField] private CameraController camController;
        [SerializeField] private BarController staminaBar;
        
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private PlayerSfx playerSfx;

        [Header("Shake Settings")] [SerializeField]
        private float fallShakeMagnitudeMultiplier = 0.05f;

        [SerializeField] private float fallShakeDurationMultiplier = 0.05f;
        [SerializeField] private float minShakeValue = 0.5f;
        
        private bool facingRight;
        private bool jumpPressed;
        private bool dashPressed;
        private bool djmpPressed;
        private bool falling;
        private bool died;
        private bool touchingFloor;
        private bool shouldTp;
        private bool glidePressed;
        private bool grabPressed;
        
        private event Action<bool> OnFlip;
        public float Speed => moveState.Speed;

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
            tlptState.onEnter += OnEnterTpHandler;
            dethState.onEnter += OnEnterDeathHandler;
            wallState.onEnter += OnEnterWallHandler;
            wallJumpState.onEnter += OnEnterWallJumpHandler;
            gldeState.onEnter += OnEnterGlideHandler;

            dethState.onExit += OnExitDeathHandler;
            spwnState.onExit += OnExitSpawnHandler;
            tlptState.onExit += OnExitTpHandler;
            gldeState.onExit += OnExitGlideHandler;

            camController.MoveCam += PausePlayer;
            camController.StopCam += ResumePlayer;
            
            InputManager.Move += OnMoveHandler;
            InputManager.Jump += OnJumpPressedHandler;
            InputManager.AbilityPress += OnAbilityPressHandler;
            InputManager.AbilityRelease += OnAbilityReleaseHandler;

            ColorSwitcher.ColorChanged += OnChangedColorHandler;
            LevelChanger.PlayerTp += OnTpHandler;

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
            dethState.onEnter -= OnEnterDeathHandler;
            wallState.onEnter -= OnEnterWallHandler;
            wallJumpState.onEnter -= OnEnterWallJumpHandler;
            gldeState.onEnter -= OnEnterGlideHandler;

            dethState.onExit -= OnExitDeathHandler;
            spwnState.onExit -= OnExitSpawnHandler;
            tlptState.onExit -= OnExitTpHandler;
            gldeState.onExit -= OnExitGlideHandler;

            camController.MoveCam -= PausePlayer;
            camController.StopCam -= ResumePlayer;
            
            InputManager.Move -= OnMoveHandler;
            InputManager.Jump -= OnJumpPressedHandler;
            InputManager.AbilityPress -= OnAbilityPressHandler;
            InputManager.AbilityRelease -= OnAbilityReleaseHandler;

            ColorSwitcher.ColorChanged -= OnChangedColorHandler;
            LevelChanger.PlayerTp -= OnTpHandler;

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

            if (fsm.CurrentState != wallState && fsm.CurrentState != wallJumpState && fsm.CurrentState != grabState)
            {
                if (facingRight && moveState.Speed < 0f || !facingRight && moveState.Speed > 0f)
                    Flip();
            }

            if (stateTxt)
                stateTxt.text = fsm.CurrentState.ID;

            if (fsm.CurrentState != moveState && fsm.CurrentState != fallState && fsm.CurrentState != jumpState &&
                fsm.CurrentState != djmpState)
                moveState.DecreaseSpeed();
            
            if (fsm.CurrentState != gldeState && fsm.CurrentState != grabState && moveState.IsGrounded())
                staminaBar.FillValue += 1.5f * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            fsm.FixedUpdate();

            if (rb.velocity.y < 0f)
            {
                if (CamShakeCheck())
                {
                    float absVel = Mathf.Abs(rb.velocity.y);

                    camController.Shake((absVel - minShakeValue) * fallShakeDurationMultiplier,
                        (absVel - minShakeValue) * fallShakeMagnitudeMultiplier);
                }
            }

            if (falling && rb.velocity.y == 0f)
                falling = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (fsm.CurrentState == fallState)
                jumpState.SpawnDust();
            
            if (!(other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Platform")))
                return;
            
            if (!moveState.IsGrounded())
                return;

            touchingFloor = true;

            falling = false;

            if (other.gameObject.TryGetComponent(out ObjMovement obj))
                obj.AddPlayer(transform);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!(other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Platform")))
                return;

            if (!moveState.IsGrounded())
                return;

            touchingFloor = true;

            falling = false;

            if (other.gameObject.TryGetComponent(out ObjMovement obj))
                obj.AddPlayer(transform);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!(other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Platform")))
                return;

            touchingFloor = false;

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
            fallState = new FallState<string>("Fall", stateSettings[2], rb, transform, this, playerSfx);
            dashState = new DashState<string>("Dash", stateSettings[3], rb, transform, this);
            djmpState = new DjmpState<string>("Djmp", stateSettings[4], this, rb, transform);
            dethState = new DeathState<string>("Death", stateSettings[5], transform, rb, this);
            spwnState = new SpawnState<string>("Spawn", stateSettings[6], transform, rb, this);
            tlptState = new TpState<string>("TP", rb);
            extpState = new ExitTpState<string>("ExitTP", rb);
            wallState = new WallState<string>("Wall", stateSettings[7], rb, transform, this, playerSfx);
            wallJumpState = new WallJumpState<string>("Wjmp", stateSettings[8], this, rb, transform);
            gldeState = new GlideState<string>("Glide", stateSettings[9], rb, transform, this, playerSfx, staminaBar);
            grabState = new GrabState<string>("Grab", stateSettings[10], rb, transform, this, playerSfx, staminaBar);
            pausState = new PauseState<string>("Pause", rb);
            edgeState = new EdgeState<string>("Edge", stateSettings[11], transform, fsmAnimController);

            fsm = new FiniteStateMachine<string>();

            fsm.AddState(idleState);
            fsm.AddState(moveState);
            fsm.AddState(jumpState);
            fsm.AddState(fallState);
            fsm.AddState(dashState);
            fsm.AddState(djmpState);
            fsm.AddState(dethState);
            fsm.AddState(spwnState);
            fsm.AddState(tlptState);
            fsm.AddState(extpState);
            fsm.AddState(wallState);
            fsm.AddState(wallJumpState);
            fsm.AddState(gldeState);
            fsm.AddState(grabState);
            fsm.AddState(pausState);
            fsm.AddState(edgeState);

            FsmTransitions();

            fsm.SetCurrentState(extpState);

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
            fsmAnimController.AddState(tlptState.ID, 8);
            fsmAnimController.AddState(extpState.ID, 9);
            fsmAnimController.AddState(wallState.ID, 10);
            fsmAnimController.AddState(wallJumpState.ID, 11);
            fsmAnimController.AddState(gldeState.ID, 12);
            fsmAnimController.AddState(grabState.ID, 13);
            fsmAnimController.AddState(edgeState.ID, 14);
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
            fsm.AddTransition(idleState, tlptState, () => shouldTp);
            fsm.AddTransition(idleState, edgeState, () => edgeState.IsOnEdge());

            fsm.AddTransition(moveState, idleState, moveState.StoppedMoving);
            fsm.AddTransition(moveState, jumpState, () => jumpPressed && moveState.IsGrounded());
            fsm.AddTransition(moveState, fallState, () => rb.velocity.y < 0 && !moveState.IsGrounded());
            fsm.AddTransition(moveState, dashState, () => dashPressed);
            fsm.AddTransition(moveState, djmpState, () => djmpPressed);
            fsm.AddTransition(moveState, dethState, () => died);
            fsm.AddTransition(moveState, tlptState, () => shouldTp);

            fsm.AddTransition(jumpState, fallState, () => rb.velocity.y < 0);
            fsm.AddTransition(jumpState, idleState,
                () => moveState.IsGrounded() && jumpState.HasJumped && rb.velocity.y <= 0f && touchingFloor);
            fsm.AddTransition(jumpState, moveState,
                () => moveState.IsGrounded() && jumpState.HasJumped && rb.velocity.y <= 0f && touchingFloor);
            fsm.AddTransition(jumpState, dashState, () => dashPressed);
            fsm.AddTransition(jumpState, djmpState, () => djmpPressed);
            fsm.AddTransition(jumpState, dethState, () => died);
            fsm.AddTransition(jumpState, tlptState, () => shouldTp);

            fsm.AddTransition(fallState, gldeState, () => glidePressed && !staminaBar.depleted);
            fsm.AddTransition(fallState, moveState, () => !falling && ShouldEnterMove() && moveState.IsGrounded());
            fsm.AddTransition(fallState, idleState, () => !falling && moveState.IsGrounded());
            fsm.AddTransition(fallState, dashState, () => dashPressed);
            fsm.AddTransition(fallState, djmpState, () => djmpPressed);
            fsm.AddTransition(fallState, jumpState, () => jumpPressed && fallState.CanCoyoteJump);
            fsm.AddTransition(fallState, dethState, () => died);
            fsm.AddTransition(fallState, tlptState, () => shouldTp);
            fsm.AddTransition(fallState, wallState, wallState.CanEnterWall);
            fsm.AddTransition(fallState, edgeState, () => rb.velocity.y >= 0f && edgeState.IsOnEdge());

            fsm.AddTransition(dashState, fallState, () => dashState.Ended);
            fsm.AddTransition(dashState, dethState, () => died);
            fsm.AddTransition(dashState, tlptState, () => shouldTp);

            fsm.AddTransition(djmpState, fallState, () => rb.velocity.y < 0f && !moveState.IsGrounded());
            fsm.AddTransition(djmpState, idleState, () => moveState.IsGrounded() && djmpState.HasJumped && rb.velocity.y <= 0f && touchingFloor);
            fsm.AddTransition(djmpState, dethState, () => died);
            fsm.AddTransition(djmpState, tlptState, () => shouldTp);

            fsm.AddTransition(dethState, spwnState, () => dethState.Ended);

            fsm.AddTransition(spwnState, idleState, () => spwnState.Ended);

            fsm.AddTransition(tlptState, extpState, () => tlptState.Ended);

            fsm.AddTransition(extpState, idleState, () => extpState.Ended);

            fsm.AddTransition(wallState, wallJumpState, () => jumpPressed);
            fsm.AddTransition(wallState, idleState, moveState.IsGrounded);
            fsm.AddTransition(wallState, fallState, () => !wallState.IsTouchingWall() || ColorSwitcher.Instance.CurrentColor != ColorSwitcher.QColor.Green);
            fsm.AddTransition(wallState, dethState, () => died);
            fsm.AddTransition(wallState, grabState, () => grabPressed && !staminaBar.depleted);

            fsm.AddTransition(wallJumpState, fallState, () => rb.velocity.y < 0);
            fsm.AddTransition(wallJumpState, idleState,
                () => moveState.IsGrounded() && wallJumpState.HasJumped && rb.velocity.y <= 0f && touchingFloor);
            fsm.AddTransition(wallJumpState, dashState, () => dashPressed);
            fsm.AddTransition(wallJumpState, djmpState, () => djmpPressed);
            fsm.AddTransition(wallJumpState, dethState, () => died);
            fsm.AddTransition(wallJumpState, tlptState, () => shouldTp);
            
            fsm.AddTransition(gldeState, fallState, () => !glidePressed || staminaBar.depleted);
            fsm.AddTransition(gldeState, idleState, () => moveState.IsGrounded());
            fsm.AddTransition(gldeState, dethState, () => died);
            fsm.AddTransition(gldeState, edgeState, edgeState.IsOnEdge);
            
            fsm.AddTransition(grabState, wallState, () => !grabPressed || staminaBar.depleted);
            fsm.AddTransition(grabState, wallJumpState, () => jumpPressed);
            fsm.AddTransition(grabState, dethState, () => died);

            fsm.AddTransition(edgeState, idleState, () => moveState.IsGrounded() && !edgeState.IsOnEdge());
            fsm.AddTransition(edgeState, moveState, ShouldEnterMove);
            fsm.AddTransition(edgeState, jumpState, () => jumpPressed && edgeState.IsOnEdge());
            fsm.AddTransition(edgeState, fallState, () => rb.velocity.y < 0 && !moveState.IsGrounded() && !edgeState.IsOnEdge());
            fsm.AddTransition(edgeState, dashState, () => dashPressed);
            fsm.AddTransition(edgeState, djmpState, () => djmpPressed);
            fsm.AddTransition(edgeState, dethState, () => died);
            fsm.AddTransition(edgeState, tlptState, () => shouldTp);
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

            wallState.FacingRight = facingRight;
            wallJumpState.FacingRight = facingRight;
            grabState.FacingRight = facingRight;

            OnFlip?.Invoke(facingRight);
        }

        /// <summary>
        /// Pause player fsm
        /// </summary>
        private void PausePlayer()
        {
            fsmAnimController.TogglePauseAnim(true);
            fsm.InterruptState(pausState);
        }
        
        /// <summary>
        /// Resume player fsm
        /// </summary>
        private void ResumePlayer()
        {
            fsmAnimController.TogglePauseAnim(false);
            fsm.StopInterrupt();
        }
        
        /// <summary>
        /// Stop teleport state
        /// </summary>
        public void EndTp()
        {
            tlptState.OnEnd();
        }

        /// <summary>
        /// Stop exit teleport state
        /// </summary>
        private void EndExitTp()
        {
            extpState.OnEnd();
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

            if (color != ColorSwitcher.QColor.Yellow)
                glidePressed = false;
            
            if (color != ColorSwitcher.QColor.Green)
                grabPressed = false;
        }

        /// <summary>
        /// Handle player Teleport action
        /// </summary>
        private void OnTpHandler()
        {
            shouldTp = true;
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
            wallState.SetInput(input.x);
            gldeState.SetInput(input.x);
            wallJumpState.SetInput(input.x);
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
        /// Handle player ability input
        /// </summary>
        /// <returns>True if player can dash</returns>
        private void OnAbilityPressHandler()
        {
            switch (ColorSwitcher.Instance.CurrentColor)
            {
                case ColorSwitcher.QColor.None:
                    break;
                case ColorSwitcher.QColor.Red:
                    OnDashHandler();
                    break;
                case ColorSwitcher.QColor.Blue:
                    OnDjmpHandler();
                    break;
                case ColorSwitcher.QColor.Green:
                    OnGrabHandler();
                    break;
                case ColorSwitcher.QColor.Yellow:
                    OnGlideHandler();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// Handle player ability release input
        /// </summary>
        /// <returns>True if player can dash</returns>
        private void OnAbilityReleaseHandler()
        {
            switch (ColorSwitcher.Instance.CurrentColor)
            {
                case ColorSwitcher.QColor.None:
                    break;
                case ColorSwitcher.QColor.Red:
                    break;
                case ColorSwitcher.QColor.Blue:
                    break;
                case ColorSwitcher.QColor.Green:
                    OnGrabReleaseHandler();
                    break;
                case ColorSwitcher.QColor.Yellow:
                    OnGlideReleaseHandler();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle player Dash input
        /// </summary>
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
        /// Handle player Grab input
        /// </summary>
        private void OnGrabHandler()
        {
            grabPressed = true;
        }

        /// <summary>
        /// Handle player Glide input
        /// </summary>
        private void OnGlideHandler()
        {
            glidePressed = true;
        }

        /// <summary>
        /// Handle player Glide release input
        /// </summary>
        private void OnGlideReleaseHandler()
        {
            glidePressed = false;
        }

        /// <summary>
        /// Handle player Grab release input
        /// </summary>
        private void OnGrabReleaseHandler()
        {
            grabPressed = false;
        }
        
        /// <summary>
        /// Handle player HAS jumped
        /// </summary>
        private void OnEnterJumpHandler()
        {
            jumpPressed = false;

            playerSfx.Jump();

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

            if (fsm.PreviousState != jumpState && fsm.PreviousState != djmpState && fsm.PreviousState != dashState &&
                fsm.PreviousState != wallJumpState && fsm.PreviousState != wallState)
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
            playerSfx.Djmp();
            djmpPressed = false;

            djmpPs.Play();
            djmpPs2.Play();
        }

        /// <summary>
        /// Handle player died
        /// </summary>
        private void OnEnterDeathHandler()
        {
            playerSfx.Death();
        }

        /// <summary>
        /// Handle player started teleport
        /// </summary>
        private void OnEnterTpHandler()
        {
            shouldTp = false;
        }

        /// <summary>
        /// Handle player entered wall
        /// </summary>
        private void OnEnterWallHandler()
        {
            if (fsm.PreviousState != grabState)
                Flip();
        }
        
        /// <summary>
        /// Handle player started wall jump
        /// </summary>
        private void OnEnterWallJumpHandler()
        {
            moveState.ResetSpeed();
        }

        /// <summary>
        /// Handle player started glide
        /// </summary>
        private void OnEnterGlideHandler()
        {
            playerSfx.PlayGlide();
            gldePs.Play();
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

        /// <summary>
        /// Handle player exited teleport
        /// </summary>
        private void OnExitTpHandler()
        {
            //LevelChanger.EndLevel();
        }

        /// <summary>
        /// Handle player exited glide
        /// </summary>
        private void OnExitGlideHandler()
        {
            playerSfx.StopGlide();
            gldePs.Stop();
        }
        
        /// <summary>
        /// Check if player about to land
        /// </summary>
        /// <returns>True if ground is near</returns>
        private bool CamShakeCheck()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, LayerMask.GetMask("Default"));

            bool grounded = hit.collider && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform"));
            return grounded;
        }
        
        public void Kill()
        {
            if (died) return;

            died = true;

            dethState.Direction = new Vector2(-moveState.Input, -rb.velocity.normalized.y);
        }
    }
}