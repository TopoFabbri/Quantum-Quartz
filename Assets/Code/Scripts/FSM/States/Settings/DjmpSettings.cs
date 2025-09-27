using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.States.Settings
{
    [CreateAssetMenu(menuName = "StateSettings/Djmp", fileName = "DjmpSettings", order = 0)]
    public class DjmpSettings : StateSettings
    {
        [HeaderPlus("Jump Settings")]
        public JumpSettings jumpSettings;

        [HeaderPlus("Double Jump Settings")]
        public float shakeDur = 0.1f;
        public float shakeMag = 0.1f;
    }
}