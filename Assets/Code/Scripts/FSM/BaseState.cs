using System;

namespace Code.Scripts.FSM
{
    public abstract class BaseState<T> : IState
    {
        public event Action onEnter;
        public event Action onExit;
        
        public T ID { get; private set; }

        protected BaseState(T id)
        {
            ID = id;
        }

        public virtual void OnEnter()
        {
            onEnter?.Invoke();
        }

        public virtual void OnUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnExit()
        {
            onExit?.Invoke();
        }
    }
}

