using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Glide", fileName = "GlideSettings", order = 0)]
    public class GlideSettings : FallSettings
    {
        [Header("Glide")]
        public float fallSpeed = .1f;
        public float staminaMitigation = .05f;
        public float initStaminaCut = 3f;
    }
}