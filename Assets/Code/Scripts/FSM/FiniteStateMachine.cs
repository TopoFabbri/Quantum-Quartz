using System;
using System.Collections.Generic;
using Patterns.FSM;

namespace Code.Scripts.FSM
{
    public class FiniteStateMachine<T>
    {
        private readonly Dictionary<T, BaseState<T>> states = new();

        public BaseState<T> CurrentState { get; private set; }
        public BaseState<T> PreviousState { get; private set; }
        public BaseState<T> InterruptedState { get; private set; }

        private readonly Dictionary<Type, List<Transition<T>>> transitions = new();
        private List<Transition<T>> currentTransitions = new();

        private static readonly List<Transition<T>> EmptyTransitions = new(0);

        private bool initialized;
        private bool interrupted;
        private readonly int maxResolveDepth;

        public event Action<T> StateChanged;

        public FiniteStateMachine(int maxResolveDepth = 1)
        {
            this.maxResolveDepth = maxResolveDepth;
        }

        public void Init()
        {
            if (!initialized)
                initialized = true;
        }

        public void Update()
        {
            Transition<T> transition = null;
            for (int resolveDepth = 0; resolveDepth < maxResolveDepth; resolveDepth++)
            {
                transition = GetTransition();

                if (transition == null)
                    break;

                SetCurrentState(transition.To);
            }

            if (initialized)
            {
                CurrentState.OnUpdate();
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
                CurrentState.OnFixedUpdate();
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
        /// Sets current state, running OnExit() methods from previous and OnEnter() from new one
        /// </summary>
        /// <param name="state"></param>
        public void SetCurrentState(BaseState<T> state)
        {
            if (CurrentState == state) return;

            CurrentState?.OnExit();

            PreviousState = CurrentState;
            CurrentState = state;

            transitions.TryGetValue(CurrentState.GetType(), out currentTransitions);

            currentTransitions ??= EmptyTransitions;

            StateChanged?.Invoke(CurrentState.ID);

            CurrentState?.OnEnter();
        }

        public void InterruptState(BaseState<T> state)
        {
            if (CurrentState == state)
                return;
            
            interrupted = true;
            
            InterruptedState = CurrentState;
            CurrentState = state;
            
            CurrentState?.OnEnter();
        }
        
        public void StopInterrupt()
        {
            CurrentState.OnExit();
            
            CurrentState = InterruptedState;
            
            interrupted = false;
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
            if (interrupted)
                return null;
            
            foreach (var transition in currentTransitions)
                if (transition.Condition())
                    return transition;

            return null;
        }
    }
}