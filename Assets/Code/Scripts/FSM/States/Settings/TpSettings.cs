using UnityEngine;

namespace Code.Scripts.States.Settings
{
    [CreateAssetMenu(menuName = "StateSettings/Tp", fileName = "TpSettings", order = 0)]
    public class TpSettings : StateSettings
    {
        public Vector2 gearOffset = new(0f, 0.5f);
    }
}