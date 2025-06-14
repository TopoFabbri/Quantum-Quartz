using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Fall", fileName = "FallSettings", order = 0)]
    public class FallSettings : StateSettings
    {
        [HeaderPlus("Move Settings")]
        public MoveSettings moveSettings;

        [HeaderPlus("Fall Settings")]
        public float coyoteTime = 0.1f;
        public float maxFallSpeed = 10f;
        public string fallSoundName = "Play_Lab_Landing";
        public GameObject dust;
        public VelocityCurve fallCurve;
    }
}