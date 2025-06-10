using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class ExitTpState<T> : SpawnState<T>
    {   
        public ExitTpState(T id, SpawnSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings, sharedContext)
        {
        }
    }
}