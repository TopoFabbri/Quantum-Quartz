using System.Collections.Generic;
using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Input;
using Code.Scripts.Platforms;
using Code.Scripts.States;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage player actions
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private FiniteStateMachine<string> fsm;

        // States
        private IdleState<string> idleState;
        private MoveState<string> moveState;
        private JumpState<string> jumpState;
        private FallState<string> fallState;
        private DashState<string> dashState;
        private DjmpState<string> djmpState;

        [SerializeField] private StateSettings.StateSettings[] stateSettings;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private TextMeshProUGUI stateTxt;
        [SerializeField] private List<GameObject> flipObjects = new();

        [SerializeField] private SpriteRenderer sprite;

        private bool facingRight;
        private bool jumpPressed;
        private bool dashPressed;
        private bool djmpPressed;
        private bool falling;

        private void Awake()
        {
            idleState = new IdleState<string>("Idle");
            moveState = new MoveState<string>("Move", stateSettings[0], rb, transform);
            jumpState = new JumpState<string>("JumpStart", stateSettings[1], this, rb, transform);
            fallState = new FallState<string>("Fall", stateSettings[2], rb, transform);
            dashState = new DashState<string>("Dash", stateSettings[3], rb, this);
            djmpState = new DjmpState<string>("MiniJump", stateSettings[4], this, rb, transform);

            fsm = new FiniteStateMachine<string>();

            fsm.AddState(idleState);
            fsm.AddState(moveState);
            fsm.AddState(jumpState);
            fsm.AddState(fallState);
            fsm.AddState(dashState);
            fsm.AddState(djmpState);

            FsmTransitions();

            fsm.SetCurrentState(idleState);

            fsm.Init();
        }

        private void OnEnable()
        {
            jumpState.onEnter += OnEnterJumpHandler;
            fallState.onEnter += OnEnterFallHandler;
            dashState.onEnter += OnEnterDashHandler;
            djmpState.onEnter += OnEnterDjmpHandler;

            InputManager.Move += OnMoveHandler;
            InputManager.Jump += OnJumpPressedHandler;
            InputManager.Dash += OnDashHandler;
            InputManager.Djmp += OnDjmpHandler;
            InputManager.Restart += OnRestartHandler;

            ColorSwitcher.ColorChanged += OnChangedColorHandler;
        }

        private void OnDisable()
        {
            jumpState.onEnter -= OnEnterJumpHandler;
            fallState.onEnter -= OnEnterFallHandler;
            dashState.onEnter -= OnEnterDashHandler;
            djmpState.onEnter -= OnEnterDjmpHandler;

            InputManager.Move -= OnMoveHandler;
            InputManager.Jump -= OnJumpPressedHandler;
            InputManager.Dash -= OnDashHandler;
            InputManager.Djmp -= OnDjmpHandler;
            InputManager.Restart -= OnRestartHandler;
            ColorSwitcher.ColorChanged -= OnChangedColorHandler;
        }

        private void Update()
        {
            fsm.Update();

            if (stateTxt)
                stateTxt.text = fsm.GetCurrentState().ID;
        }

        private void FixedUpdate()
        {
            fsm.FixedUpdate();

            if (falling && rb.velocity.y == 0f)
                falling = false;

            if (facingRight && moveState.Input < 0f || !facingRight && moveState.Input > 0f)
                Flip();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Floor"))
                return;

            if (!moveState.IsGrounded())
                return;

            falling = false;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        /// <summary>
        /// Add fsm transitions
        /// </summary>
        private void FsmTransitions()
        {
            fsm.AddTransition(idleState, moveState, () => moveState.Input != 0);
            fsm.AddTransition(idleState, jumpState, () => jumpPressed);
            fsm.AddTransition(idleState, fallState, () => rb.velocity.y < 0);
            fsm.AddTransition(idleState, dashState, () => dashPressed);
            fsm.AddTransition(idleState, djmpState, () => djmpPressed);

            fsm.AddTransition(moveState, idleState, moveState.StoppedMoving);
            fsm.AddTransition(moveState, jumpState, () => jumpPressed);
            fsm.AddTransition(moveState, fallState, () => rb.velocity.y < 0);
            fsm.AddTransition(moveState, dashState, () => dashPressed);
            fsm.AddTransition(moveState, djmpState, () => djmpPressed);

            fsm.AddTransition(jumpState, fallState, () => rb.velocity.y < 0);
            fsm.AddTransition(jumpState, idleState, moveState.IsGrounded);
            fsm.AddTransition(jumpState, dashState, () => dashPressed);
            fsm.AddTransition(jumpState, djmpState, () => djmpPressed);

            fsm.AddTransition(fallState, moveState, () => !falling);
            fsm.AddTransition(fallState, dashState, () => dashPressed);
            fsm.AddTransition(fallState, djmpState, () => djmpPressed);

            fsm.AddTransition(dashState, fallState, () => dashState.Ended);

            fsm.AddTransition(djmpState, fallState, () => !djmpState.JumpAvailable);
        }

        /// <summary>
        /// Flip character
        /// </summary>
        private void Flip()
        {
            if (fsm.GetCurrentState() == dashState)
                return;

            facingRight = !facingRight;
            sprite.flipX = !sprite.flipX;
            dashState.Flip();

            foreach (GameObject flipObject in flipObjects)
            {
                flipObject.transform.Rotate(0f, 0f, 180f);
                flipObject.transform.localPosition = new Vector3(-flipObject.transform.localPosition.x,
                    flipObject.transform.localPosition.y, flipObject.transform.localPosition.z);
            }
        }

        /// <summary>
        /// Manage player actions when color is changed
        /// </summary>
        /// <param name="color">New color</param>
        private void OnChangedColorHandler(ColorSwitcher.QColors color)
        {
            if (color != ColorSwitcher.QColors.Red)
                dashState.Reset();

            if (color != ColorSwitcher.QColors.Blue)
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
        }

        /// <summary>
        /// Handle player jump action
        /// </summary>
        private void OnJumpPressedHandler()
        {
            if (moveState.IsGrounded())
                jumpPressed = true;
        }

        /// <summary>
        /// Handle player dash input
        /// </summary>
        /// <returns>True if player can dash</returns>
        private void OnDashHandler()
        {
            if (dashState.DashAvailable && ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColors.Red)
                dashPressed = true;
        }

        /// <summary>
        /// Handle player MiniJump input
        /// </summary>
        private void OnDjmpHandler()
        {
            if (djmpState.JumpAvailable && ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColors.Blue)
                djmpPressed = true;
        }

        /// <summary>
        /// Handle player HAS jumped
        /// </summary>
        private void OnEnterJumpHandler()
        {
            jumpPressed = false;
        }

        /// <summary>
        /// Handle player started falling
        /// </summary>
        private void OnEnterFallHandler()
        {
            falling = true;
        }

        /// <summary>
        /// Handle player started dashing
        /// </summary>
        private void OnEnterDashHandler()
        {
            dashPressed = false;
        }

        /// <summary>
        /// Handle player started Djump
        /// </summary>
        private void OnEnterDjmpHandler()
        {
            djmpPressed = false;
        }

        /// <summary>
        /// Handle restart input
        /// </summary>
        private static void OnRestartHandler()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}