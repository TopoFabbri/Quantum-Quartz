using Code.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.States.Settings
{
    [CreateAssetMenu(menuName = "StateSettings/DashSettings", fileName = "DashSettings", order = 0)]
    public class DashSettings : StateSettings
    {
        [HeaderPlus("Dash Settings")]
        public float speed = 10f;
        public float duration = 0.5f;
        public float cooldown = 3f;
        public float shakeDur = 0.1f;
        public float shakeMag = 0.1f;
        public float staminaRegenSpeed = .35f;
        public float staminaFloorRegenSpeed = 1.5f;
        public float staminaMitigationAmount = 100f;
        
        [HeaderPlus("Dash Wall Check")]
        public float wallCheckDis;
        public List<string> tags;
    }
}