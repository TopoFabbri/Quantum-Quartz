using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Glide", fileName = "GlideSettings", order = 0)]
    public class GlideSettings : FallSettings
    {
        [FormerlySerializedAs("gravScale")] [Header("Glide")]
        public float fallSpeed = .1f;
    }
}