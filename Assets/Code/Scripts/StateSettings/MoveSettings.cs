using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Move", fileName = "MoveSettings", order = 0)]
    public class MoveSettings : StateSettings
    {
        [Header("Movement:")]
        public float accel = 5f;
        public float maxSpeed = 5f;
        public float minSpeed = 0.5f;
        public float groundFriction = 1f;
        public float airFriction;
        
        [Header("Ground Check:")]
        public Vector2 groundCheckOffset;
        public float groundCheckRadius;
        public LayerMask groundLayer;
        public bool shouldDraw;
        
        [Header("Wall Check:")]
        public float wallCheckDis;
        public Vector2 wallCheckSize;
    }
}