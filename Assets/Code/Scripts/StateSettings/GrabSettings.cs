using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Grab", fileName = "GrabSettings", order = 0)]
    public class GrabSettings : StateSettings
    {
        [HeaderPlus("Wall Settings")]
        public WallSettings wallSettings;

        [HeaderPlus("Grab Settings")]
        public float staminaMitigation = .2f;
        public float initStaminaCut = .5f;
        public float staminaRegenSpeed = 1.5f;
    }
}