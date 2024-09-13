using System.Collections;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class WjmpState<T> : JumpState<T>
    {
        protected WjmpSettings WjmpSettings => settings as WjmpSettings;
        
        private bool canMove;
        
        public WjmpState(T id, StateSettings.StateSettings stateSettings, MonoBehaviour mb, Rigidbody2D rb, Transform transform) : base(id, stateSettings, mb, rb, transform)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            canMove = false;
            mb.StartCoroutine(WaitAndReturnInput(WjmpSettings.noInputTime));
        }

        public override void OnUpdate()
        {
            if (!canMove)
                Input = 0f;
            
            base.OnUpdate();
        }

        /// <summary>
        /// Stop input for given time
        /// </summary>
        /// <param name="noInputTime">Time to wait</param>
        /// <returns></returns>
        private IEnumerator WaitAndReturnInput(float noInputTime)
        {
            yield return new WaitForSeconds(noInputTime);
            canMove = true;
        }
    }
}