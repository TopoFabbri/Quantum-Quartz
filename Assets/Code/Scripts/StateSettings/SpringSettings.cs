using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.StateSettings
{
    [CreateAssetMenu(menuName = "StateSettings/Spring", fileName = "SpringSettings", order = 0)]
    public class SpringSettings : StateSettings
    {
        [HeaderPlus("Move Settings")]
        public MoveSettings moveSettings;
    }
}