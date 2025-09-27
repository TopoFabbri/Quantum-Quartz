using Code.Scripts.FSM;

namespace Code.Scripts.States
{
    /// <summary>
    /// Idle state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdleState<T> : BaseState<T>
    {
        public IdleState(T id) : base(id)
        {
        }
    }
}
