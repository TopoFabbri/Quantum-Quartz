using System;

namespace Code.Scripts.FSM
{
    public class Transition<T>
    {
        public Func<bool> Condition { get; }
        public BaseState<T> To { get; }

        public Transition(BaseState<T> to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }
}

