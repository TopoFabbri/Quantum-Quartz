using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Falling state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FallState<T> : MoveState<T>
    {
        public FallState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform) : base(id, stateSettings, rb, transform)
        {
            settings = stateSettings;
            moveSettings = (settings as FallSettings)?.moveSettings;
        }
    }
}