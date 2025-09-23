using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.States.Settings
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
    }
}