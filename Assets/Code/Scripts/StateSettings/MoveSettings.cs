using Code.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Move", fileName = "MoveSettings", order = 0)]
    public class MoveSettings : StateSettings
    {
        [HeaderPlus("Movement")]
        public float accel = 5f;
        public float maxSpeed = 5f;
        public float minSpeed = 0.5f;
        public float groundFriction = 1f;
        public float airFriction;
        
        [HeaderPlus("Wall Check")]
        public float wallCheckDis;
        public List<string> tags;
    }
}