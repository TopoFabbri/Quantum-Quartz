using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Death", fileName = "DeathSettings", order = 0)]
    public class DeathSettings : StateSettings
    {
        public float accel;
        public float maxSpeed;
        public float duration;
        public float movingDuration;
    }
}