using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Djmp", fileName = "DjmpSettings", order = 0)]
    public class DjmpSettings : JumpSettings
    {
        public float shakeDur = 0.1f;
        public float shakeMag = 0.1f;
    }
}