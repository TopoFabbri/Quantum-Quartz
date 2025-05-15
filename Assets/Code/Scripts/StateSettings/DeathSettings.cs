using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Death", fileName = "DeathSettings", order = 0)]
    public class DeathSettings : StateSettings
    {
        [HeaderPlus("Death Settings")]
        public float accel;
        public float maxSpeed;
        public float duration;
        public float movingDuration;
        public float shakeDur = 0.5f;
        public float shakeMag = 0.3f;
    }
}