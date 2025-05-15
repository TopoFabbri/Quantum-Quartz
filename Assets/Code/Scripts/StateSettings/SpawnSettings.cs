using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Spawn", fileName = "SpawnSettings", order = 0)]
    public class SpawnSettings : StateSettings
    {
        [HeaderPlus("Spawn Settings")]
        public float duration;
        public float height;
    }
}