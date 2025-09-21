using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.States.Settings
{
    [CreateAssetMenu(menuName = "StateSettings/Spring", fileName = "SpringSettings", order = 0)]
    public class SpringSettings : StateSettings
    {
        [HeaderPlus("Move Settings")]
        public MoveSettings moveSettings;

        [HeaderPlus("Fall Settings")]
        public FallSettings fallSettings;

        [HeaderPlus("Spring Settings")]
        public VelocityCurve springCurve;
    }
}