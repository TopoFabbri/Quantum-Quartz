using System;
using System.Collections;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class JumpStartState<T> : MoveState<T>
    {
        private JumpStartSettings JumpSettings => settings as JumpStartSettings;
        
        private readonly MonoBehaviour mb;
        
        public JumpStartState(T id, StateSettings.StateSettings stateSettings, MonoBehaviour mb, Rigidbody2D rb) : base(id, stateSettings, rb)
        {
            settings = stateSettings;
            moveSettings = JumpSettings.moveSettings;
            
            this.mb = mb;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            mb.StartCoroutine(JumpOnFU());
        }
        
        private IEnumerator JumpOnFU()
        {
            yield return new WaitForFixedUpdate();
            
            rb.AddForce(JumpSettings.jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
    }
}