using System;
using System.Collections.Generic;
using Patterns.FSM;

namespace Code.Scripts.FSM
{
    public class FiniteStateMachine<T>
    {
        public class TransitionContext
        {
            public IReadOnlyCollection<string> Results { get { return results; } }

            private readonly HashSet<string> results;
            private readonly Dictionary<string, Func<bool>> conditions;

            public TransitionContext()
            {
                results = new HashSet<string>();
                conditions = new Dictionary<string, Func<bool>>();
            }

            public void AddCondition(string name, Func<bool> condition)
            {
                conditions.Add(name, condition);
            }

            public void RecalculateConditions()
            {
                results.Clear();
                foreach (KeyValuePair<string, Func<bool>> keyValue in conditions)
                {
                    if (keyValue.Value())
                    {
                        results.Add(keyValue.Key);
                    }
                }
            }

            public bool HasCondition(string name) => conditions.ContainsKey(name);

            public bool IsTrue(string name) => results.Contains(name);
            public bool IsTrue(IEnumerable<string> names) => results.IsSupersetOf(names);

            public bool IsFalse(string name) => !results.Contains(name);
            public bool IsFalse(IEnumerable<string> names) => !results.Overlaps(names); 
        }

        private readonly Dictionary<T, BaseState<T>> states = new();

        public BaseState<T> CurrentState { get; private set; }
        public BaseState<T> PreviousState { get; private set; }
        public BaseState<T> InterruptedState { get; private set; }
        public TransitionContext Context { get; private set; }

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
            Context = new TransitionContext();
        }

        public void Init()
        {
            if (!initialized)
                initialized = true;
        }

        public void Update()
        {
            Transition<T> transition;
            for (int resolveDepth = 0; resolveDepth < maxResolveDepth; resolveDepth++)
            {
                Context.RecalculateConditions();
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

        public void AddGlobalTransition(BaseState<T> to, Func<bool> condition)
        {
            foreach (KeyValuePair<T, BaseState<T>> keyValue in states)
            {
                BaseState<T> state = keyValue.Value;
                if (!state.ID.Equals(to.ID))
                {
                    AddTransition(state, to, condition);
                }
            }
        }

        public void AddTransition(BaseState<T> from, BaseState<T> to, string[] trueConditions, string[] falseConditions = null)
        {
            // Clean up trueConditions array
            if ((trueConditions?.Length ?? 0) != 0)
            {
                List<string> validValues = new List<string>();
                for (int i = 0; i < trueConditions.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(trueConditions[i]) && Context.HasCondition(trueConditions[i]))
                    {
                        validValues.Add(trueConditions[i]);
                    }
                }
                trueConditions = validValues.ToArray();
            }

            // Clean up falseConditions array
            if ((falseConditions?.Length ?? 0) != 0)
            {
                List<string> validValues = new List<string>();
                for (int i = 0; i < falseConditions.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(falseConditions[i]) && Context.HasCondition(falseConditions[i]))
                    {
                        validValues.Add(falseConditions[i]);
                    }
                }
                falseConditions = validValues.ToArray();
            }

            // Create the transition based on whether trueConditions and/or falseConditions contain elements
            if ((trueConditions?.Length ?? 0) == 0)
            {
                if ((falseConditions?.Length ?? 0) == 0)
                {
                    // No true or false conditions
                    return;
                }
                else
                {
                    // Only false conditions
                    AddTransition(from, to, () => Context.IsFalse(falseConditions));
                }
            }
            else
            {
                if ((falseConditions?.Length ?? 0) == 0)
                {
                    // Only true conditions
                    AddTransition(from, to, () => Context.IsTrue(trueConditions));
                }
                else
                {
                    // True and false conditions
                    AddTransition(from, to, () => Context.IsTrue(trueConditions) && Context.IsFalse(falseConditions));
                }
            }
        }

        public void AddGlobalTransition(BaseState<T> to, string[] trueConditions, string[] falseConditions = null)
        {
            foreach (KeyValuePair<T, BaseState<T>> keyValue in states)
            {
                BaseState<T> state = keyValue.Value;
                if (!state.ID.Equals(to.ID))
                {
                    AddTransition(state, to, trueConditions, falseConditions);
                }
            }
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