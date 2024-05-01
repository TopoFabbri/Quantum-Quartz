using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/DashSettings", fileName = "DashSettings", order = 0)]
    public class DashSettings : StateSettings
    {
        public float speed = 10f;
        public float duration = 0.5f;
    }
}