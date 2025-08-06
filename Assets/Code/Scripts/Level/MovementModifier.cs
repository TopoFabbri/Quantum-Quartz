using Code.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Level
{
    [CreateAssetMenu(menuName = "Custom/MovementModifier", fileName = "MovementModifier", order = 0)]
    public class MovementModifier : ScriptableObject
    {
        [HeaderPlus("Movement Multipliers")]
        public float accel = 1;
        public float maxSpeed = 1;
        public float minSpeed = 1;
        public float groundFriction = 1;
        public float airFriction = 1;
    }
}
