using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Fall", fileName = "FallSettings", order = 0)]
    public class FallSettings : StateSettings
    {
        public MoveSettings moveSettings;

        public float coyoteTime = 0.1f;
    }
}