using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Jump", fileName = "JumpSettings", order = 0)]
    public class JumpSettings : StateSettings
    {
        [HeaderPlus("Move Settings")]
        public MoveSettings moveSettings;

        [HeaderPlus("Jump Settings")]
        public float jumpForce = 10f;
        public GameObject dust;
    }
}