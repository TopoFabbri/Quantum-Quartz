using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Move", fileName = "MoveSettings", order = 0)]
    public class MoveSettings : StateSettings
    {
        public float accel = 5f;
        public float maxSpeed = 5f;
        public float groundFriction = 1f;
        public float airFriction;
        
        public Vector2 groundCheckOffset;
        public float groundCheckRadius;
        public LayerMask groundLayer;
    }
}