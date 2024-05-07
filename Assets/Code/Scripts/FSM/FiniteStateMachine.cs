using System;
using System.Collections.Generic;
using Patterns.FSM;

namespace Code.Scripts.FSM
{
    public class FiniteStateMachine<T>
    {
        private readonly Dictionary<T, BaseState<T>> states = new();
        private BaseState<T> currentState;

        private readonly Dictionary<Type, List<Transition<T>>> transitions = new();
        private List<Transition<T>> currentTransitions = new();

        private static readonly List<Transition<T>> EmptyTransitions = new(0);

        private bool initialized;

        public event Action<T> StateChanged; 
        
        public void Init()
        {
            if (!initialized)
                initialized = true;
        }

        public void Update()
        {
            Transition<T> transition = GetTransition();
 
            if (transition != null)
                SetCurrentState(transition.To);

            if (initialized)
            {
                currentState.OnUpdate();
            }
            else
            {
                throw new Exception("FSM not initialized");
            }
        }

        public void FixedUpdate()
        {
            if (initialized)
            {
                currentState.OnFixedUpdate();
            }
            else
            {
                throw new Exception("FSM not initialized");
            }
        }

        /// <summary>
        /// Adds newState to fsm's dictionary
        /// </summary>
        /// <param name="newState"></param>
        public void AddState(BaseState<T> newState)
        {
            states.Add(newState.ID, newState);
        }

        /// <summary>
        /// Gets state from fsm's dictionary
        /// </summary>
        /// <param name="stateID"></param>
        /// <returns></returns>
        public BaseState<T> GetState(T stateID)
        {
            if (states.ContainsKey(stateID))
                return states[stateID];
            return null;
        }

        /// <summary>
        /// Gets current state
        /// </summary>
        /// <returns></returns>
        public BaseState<T> GetCurrentState()
        {
            if (currentState != null)
                return currentState;

            return null;
        }

        /// <summary>
        /// Sets current state, running OnExit() methods from previous and OnEnter() from new one
        /// </summary>
        /// <param name="state"></param>
        public void SetCurrentState(BaseState<T> state)
        {
            if (currentState == state) return;

            currentState?.OnExit();

            currentState = state;

            transitions.TryGetValue(currentState.GetType(), out currentTransitions);
            
            currentTransitions ??= EmptyTransitions;

            StateChanged?.Invoke(currentState.ID);
            
            currentState?.OnEnter();
        }

        public void AddTransition(BaseState<T> from, BaseState<T> to, Func<bool> condition)
        {
            if (this.transitions.TryGetValue(from.GetType(), out List<Transition<T>> transitions) == false)
            {
                transitions = new List<Transition<T>>();
                this.transitions[from.GetType()] = transitions;
            }

            transitions.Add(new Transition<T>(to, condition));
        }

        private Transition<T> GetTransition()
        {
            foreach (var transition in currentTransitions)
                if (transition.Condition())
                    return transition;

            return null;
        }
    }
}

