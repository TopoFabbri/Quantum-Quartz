using Code.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Global", fileName = "GlobalSettings", order = 0)]
    public class GlobalSettings : StateSettings
    {
        [HeaderPlus("Ground Check")]
        public Vector2 groundCheckOffset;
        public float groundCheckDistance;
        public float edgeCheckDis = 0.3f;
        public float edgeCheckLength = 0.3f;
        public LayerMask groundLayer;
        public float minGroundDist = 0.03f;

        [HeaderPlus("Wall Check")]
        public float greenWallDelay = 0.1f;
        public float wallCheckDis;
        public List<string> wallTags;

        [HeaderPlus("Movement Settings")]
        public float neutralSpeed = 0.001f;

        [HeaderPlus("Jump Settings")]
        public float jumpBufferTime = 0.1f;

        [HeaderPlus("Debug")]
        public bool shouldDraw;
    }
}