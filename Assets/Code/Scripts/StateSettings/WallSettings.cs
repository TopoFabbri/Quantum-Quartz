using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Wall", fileName = "WallSettings", order = 0)]
    public class WallSettings : FallSettings
    {
        [Header("Wall")]
        public float gravMultiplier = .5f;
        public float upwardsGravMultiplier = 1.5f;
        public float wallDis = 0.5f;
        public float maxFallSpeed = 10f;
        public float dustDelay = 0.3f;
        public float dustOffset = 0.3f;
        public int dustQty = 4;
        public GameObject dust;
    }
}