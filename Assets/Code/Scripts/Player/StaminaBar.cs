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

        private readonly float regenSpeed;
        private readonly float depleteSpeed;
        private readonly float initCut;

        private readonly List<Func<bool>> noRegenConditions = new();

        public StaminaBar(float regenSpeed, float depleteSpeed, float initCut)
        {
            this.regenSpeed = regenSpeed;
            this.depleteSpeed = depleteSpeed;
            this.initCut = initCut;
            
            FillValue = 1f;
            AddNoRegenCondition(() => used);
        }

        public void LateUpdate()
        {
            if (!noRegenConditions.Any(noRegenCondition => noRegenCondition()))
                Regen();
            
            used = false;
        }
        
        public void Use()
        {
            if (Depleted) return;

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
        
        public void AddNoRegenCondition(Func<bool> noRegenCondition)
        {
            noRegenConditions.Add(noRegenCondition);
        }
        
        public void RemoveNoRegenCondition(Func<bool> noRegenCondition)
        {
            noRegenConditions.Remove(noRegenCondition);
        }

        private void Regen()
        {
            FillValue += regenSpeed * Time.deltaTime;

            if (!(FillValue >= 1f)) return;
            
            FillValue = 1f;
            Depleted = false;
        }
    }
}