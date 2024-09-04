using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Wall", fileName = "WallSettings", order = 0)]
    public class WallSettings : StateSettings
    {
        public FallSettings fallSettings;
        
        public float wallCheckDis = 0.2f;
        public float gravMultiplier = .5f;
    }
}