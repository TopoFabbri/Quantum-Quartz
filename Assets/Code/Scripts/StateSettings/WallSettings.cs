using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Wall", fileName = "WallSettings", order = 0)]
    public class WallSettings : StateSettings
    {
        [HeaderPlus("Fall Settings")]
        public FallSettings fallSettings;


        [HeaderPlus("Wall Settings")]
        public float wallCooldown = 0.1f;
        public float gravMultiplier = .5f;
        public float upwardsGravMultiplier = 1.5f;
        public float wallDis = 0.5f;
        public float dustDelay = 0.3f;
        public float dustOffset = 0.3f;
        public int dustQty = 4;
        public GameObject dust;
    }
}