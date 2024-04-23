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
        
        [SerializeField] private StateSettings.StateSettings[] stateSettings;
        [SerializeField] private Rigidbody2D rb;
        
        private void Awake()
        {
            idleState = new IdleState<string>("Idle");
            moveState = new MoveState<string>("Move", stateSettings[0], rb);
            
            fsm = new FiniteStateMachine<string>();
            
            fsm.AddState(idleState);
            fsm.AddState(moveState);
            
            fsm.AddTransition(idleState, moveState, () => moveState.Input != 0);
            fsm.AddTransition(moveState, idleState, () => rb.velocity.x == 0);
            
            fsm.SetCurrentState(idleState);
            
            fsm.Init();
        }

        private void OnEnable()
        {
            InputManager.Move += UpdateInput;
        }
        
        private void OnDisable()
        {
            InputManager.Move -= UpdateInput;
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
        /// Update player input on movement states
        /// </summary>
        /// <param name="input">Input value</param>
        private void UpdateInput(Vector2 input)
        {
            moveState.UpdateInput(input.x);
        }
    }
}
