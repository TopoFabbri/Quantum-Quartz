using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Grab", fileName = "GrabSettings", order = 0)]
    public class GrabSettings : WallSettings
    {
        [Header("Grab")]
        public float staminaMitigation = .2f;
    }
}