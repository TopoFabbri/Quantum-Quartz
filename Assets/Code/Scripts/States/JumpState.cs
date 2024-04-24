using System;
using System.Collections;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Jump up state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JumpState<T> : MoveState<T>
    {
        private JumpSettings JumpSettings => settings as JumpSettings;

        private readonly MonoBehaviour mb;
        private readonly Transform transform;
        
        public JumpState(T id, StateSettings.StateSettings stateSettings, MonoBehaviour mb, Rigidbody2D rb, Transform transform) : base(id, stateSettings, rb, transform)
        {
            settings = stateSettings;
            moveSettings = JumpSettings.moveSettings;
            this.transform = transform;
            
            this.mb = mb;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (!GroundCheck())
                return;
            
            mb.StartCoroutine(JumpOnFU());
        }
        
        /// <summary>
        /// Wait for fixed update and jump
        /// </summary>
        /// <returns></returns>
        private IEnumerator JumpOnFU()
        {
            yield return new WaitForFixedUpdate();
            
            rb.AddForce(JumpSettings.jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
    }
}