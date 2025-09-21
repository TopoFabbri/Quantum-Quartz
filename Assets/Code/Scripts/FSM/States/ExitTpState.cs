using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.States.Settings;

namespace Code.Scripts.States
{
    public class ExitTpState<T> : SpawnState<T>, IUnsafe
    {   
        public ExitTpState(T id, SpawnSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings, sharedContext)
        {
        }
    }
}