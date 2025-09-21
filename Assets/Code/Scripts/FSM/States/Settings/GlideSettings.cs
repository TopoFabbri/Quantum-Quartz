using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.States.Settings
{
    [CreateAssetMenu(menuName = "StateSettings/Glide", fileName = "GlideSettings", order = 0)]
    public class GlideSettings : StateSettings
    {
        [HeaderPlus("Fall Settings")]
        public FallSettings fallSettings;

        [HeaderPlus("Glide Settings")]
        public float fallSpeed = .1f;
        public float staminaMitigation = .05f;
        public float initStaminaCut = 3f;
        public float regenSpeed = 1.5f;
    }
}