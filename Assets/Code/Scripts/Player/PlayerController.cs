using System;
using Code.Scripts.FSM;
using Code.Scripts.Input;
using Code.Scripts.States;
using UnityEngine;

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
        
        [SerializeField] private StateSettings.StateSettings[] stateSettings;
        [SerializeField] private Rigidbody2D rb;

        private bool jumpPressed;
        private bool falling;
        
        private void Awake()
        {
            idleState = new IdleState<string>("Idle");
            moveState = new MoveState<string>("Move", stateSettings[0], rb, transform);
            jumpState = new JumpState<string>("JumpStart", stateSettings[1], this, rb, transform);
            fallState = new FallState<string>("Fall", stateSettings[2], rb, transform);
            
            fsm = new FiniteStateMachine<string>();
            
            fsm.AddState(idleState);
            fsm.AddState(moveState);
            fsm.AddState(jumpState);
            fsm.AddState(fallState);
            
            fsm.AddTransition(idleState, moveState, () => moveState.Input != 0);
            fsm.AddTransition(idleState, jumpState, () => jumpPressed);
            fsm.AddTransition(idleState, fallState, () => rb.velocity.y < 0);
            
            fsm.AddTransition(moveState, idleState, () => rb.velocity.x == 0);
            fsm.AddTransition(moveState, jumpState, () => jumpPressed);
            fsm.AddTransition(moveState, fallState, () => rb.velocity.y < 0);

            fsm.AddTransition(jumpState, fallState, () => rb.velocity.y <= 0);
            
            fsm.AddTransition(fallState, idleState, () => !falling);
            
            fsm.SetCurrentState(idleState);
            
            fsm.Init();
        }

        private void OnEnable()
        {
            jumpState.onEnter += OnEnterJumpHandler;
            fallState.onEnter += OnEnterFallHandler;
            InputManager.Move += OnMoveHandler;
            InputManager.Jump += OnJumpPressedHandler;
        }
        
        private void OnDisable()
        {
            jumpState.onEnter -= OnEnterJumpHandler;
            fallState.onEnter -= OnEnterFallHandler;
            InputManager.Move -= OnMoveHandler;
            InputManager.Jump -= OnJumpPressedHandler;
        }

        private void Update()
        {
            fsm.Update();
        }

        private void FixedUpdate()
        {
            fsm.FixedUpdate();
            
            if (moveState.IsGrounded() && rb.velocity.y < 0f)
                rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            falling = false;
            
            if (other.gameObject.CompareTag("Floor"))
                rb.velocity = new Vector2(rb.velocity.x, 0f);
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
            jumpPressed = true;
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
    }
}
