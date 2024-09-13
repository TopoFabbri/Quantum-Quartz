using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Jump", fileName = "JumpSettings", order = 0)]
    public class JumpSettings : MoveSettings
    {
        [Header("Jump Settings")]
        public float jumpForce = 10f;
        public float bufferTime = 0.1f;
    }
}