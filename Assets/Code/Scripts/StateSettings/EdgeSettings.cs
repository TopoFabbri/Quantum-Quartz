using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Edge", fileName = "EdgeSettings", order = 0)]
    public class EdgeSettings : StateSettings
    {
        [Header("Edge")]
        public float edgeCheckDis = 0.3f;
        public float edgeCheckLength = 0.3f;
        public Vector2 edgeCheckOffset = new Vector3(0f, -0.5f);
        public bool shouldDraw;
        public LayerMask edgeLayer;
    }
}