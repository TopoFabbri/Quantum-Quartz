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
        public float groundCheckRadius;
        public LayerMask groundLayer;
        public bool shouldDraw;


        [HeaderPlus("Jump Settings")]
        public float jumpBufferTime = 0.1f;
    }
}