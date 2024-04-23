using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/JumpStart", fileName = "JumpStartSettings", order = 0)]
    public class JumpStartSettings : StateSettings
    {
        public MoveSettings moveSettings;
        
        public float jumpForce = 10f;
    }
}