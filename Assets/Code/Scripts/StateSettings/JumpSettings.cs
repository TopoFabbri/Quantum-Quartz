using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/JumpStart", fileName = "JumpStartSettings", order = 0)]
    public class JumpSettings : StateSettings
    {
        public MoveSettings moveSettings;
        
        public float jumpForce = 10f;
        public Vector2 groundCheckOffset;
        public float groundCheckRadius;
        public LayerMask groundLayer;
    }
}