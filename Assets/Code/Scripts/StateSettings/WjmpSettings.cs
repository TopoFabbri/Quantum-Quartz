using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/WallJump", fileName = "WallJumpSettings", order = 0)]
    public class WjmpSettings : StateSettings
    {
        [HeaderPlus("Jump Settings")]
        public JumpSettings jumpSettings;

        [HeaderPlus("Wall Jump Settings")]
        public float noInputTime = 0.4f;
        public float wallJumpForce = 10f;
    }
}