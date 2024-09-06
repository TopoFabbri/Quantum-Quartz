using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Fall", fileName = "FallSettings", order = 0)]
    public class FallSettings : MoveSettings
    {
        [Header("Fall")]
        public float coyoteTime = 0.1f;
        public string fallSoundName = "Play_Lab_Landing";
    }
}