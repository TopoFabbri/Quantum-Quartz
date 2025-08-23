using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class StaminaBar
    {
        private bool used;
        public bool Depleted { get; private set; }
        public bool Full => FillValue >= 1f;
        public float FillValue { get; private set; }

        private readonly float defaultRegenSpeed;
        private readonly float depleteSpeed;
        private readonly float initCut;

        private readonly List<Func<float?>> conditionalRegenSpeeds = new();

        public StaminaBar(float regenSpeed, float depleteSpeed, float initCut)
        {
            this.defaultRegenSpeed = regenSpeed;
            this.depleteSpeed = depleteSpeed;
            this.initCut = initCut;
            
            FillValue = 1f;
            AddConditionalRegenSpeed(() => used ? 0 : null);
        }

        public void LateUpdate()
        {
            Regen(conditionalRegenSpeeds.Aggregate(float.MaxValue, (min, next) => Mathf.Min(min, next() ?? float.MaxValue)));
            used = false;
        }
        
        public void Use()
        {
            if (Depleted)
                return;

            FillValue -= depleteSpeed * Time.deltaTime;
            
            if (FillValue <= 0f)
            {
                Depleted = true;
                FillValue = 0f;
                return;
            }
            
            Depleted = false;
            used = true;
        }
        
        public void FirstUseCut()
        {
            FillValue -= initCut * depleteSpeed;
        }
        
        public void AddConditionalRegenSpeed(Func<float?> conditionalRegenSpeed)
        {
            conditionalRegenSpeeds.Add(conditionalRegenSpeed);
        }
        
        public void RemoveConditionalRegenSpeed(Func<float?> conditionalRegenSpeed)
        {
            conditionalRegenSpeeds.Remove(conditionalRegenSpeed);
        }

        private void Regen(float speed)
        {
            if (speed == float.MaxValue)
                speed = defaultRegenSpeed;

            FillValue += speed * Time.deltaTime;

            if (FillValue >= 1f)
            {
                FillValue = 1f;
                Depleted = false;
            }
        }
    }
}