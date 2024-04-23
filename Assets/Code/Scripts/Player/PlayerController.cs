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
        private JumpStartState<string> jumpStartState;
        
        [SerializeField] private StateSettings.StateSettings[] stateSettings;
        [SerializeField] private Rigidbody2D rb;

        private bool jumpPressed;
        
        private void Awake()
        {
            idleState = new IdleState<string>("Idle");
            moveState = new MoveState<string>("Move", stateSettings[0], rb);
            jumpStartState = new JumpStartState<string>("JumpStart", stateSettings[1], this, rb);
            
            fsm = new FiniteStateMachine<string>();
            
            fsm.AddState(idleState);
            fsm.AddState(moveState);
            fsm.AddState(jumpStartState);
            
            fsm.AddTransition(idleState, moveState, () => moveState.Input != 0);
            fsm.AddTransition(idleState, jumpStartState, () => jumpPressed);
            
            fsm.AddTransition(moveState, idleState, () => rb.velocity.x == 0);
            fsm.AddTransition(moveState, jumpStartState, () => jumpPressed);
            
            fsm.AddTransition(jumpStartState, idleState, () => rb.velocity.y == 0);
            
            fsm.SetCurrentState(idleState);
            
            fsm.Init();
        }

        private void OnEnable()
        {
            jumpStartState.onEnter += OnEnterJumpHandler;
            InputManager.Move += OnMoveHandler;
            InputManager.Jump += OnJumpPressedHandler;
        }
        
        private void OnDisable()
        {
            jumpStartState.onEnter -= OnEnterJumpHandler;
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
        }

        /// <summary>
        /// Handle player move action
        /// </summary>
        /// <param name="input">Input value</param>
        private void OnMoveHandler(Vector2 input)
        {
            moveState.UpdateInput(input.x);
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
    }
}
