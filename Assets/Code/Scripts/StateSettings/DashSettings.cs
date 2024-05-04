using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/DashSettings", fileName = "DashSettings", order = 0)]
    public class DashSettings : StateSettings
    {
        [Header("Dash:")]
        public float speed = 10f;
        public float duration = 0.5f;
        
        [Header("Wall Check:")]
        public float wallCheckDis;
        public Vector2 wallCheckSize;
    }
}