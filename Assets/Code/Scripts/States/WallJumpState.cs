using System.Collections;
using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class WallJumpState<T> : JumpState<T>
    {
        protected readonly WjmpSettings wjmpSettings;
        
        public WallJumpState(T id, WjmpSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.jumpSettings, sharedContext)
        {
            this.wjmpSettings = stateSettings;
        }

        public override void OnEnter()
        {
            sharedContext.Rigidbody.velocity = Vector2.zero;
            
            base.OnEnter();

            sharedContext.Rigidbody.velocity = new Vector2(sharedContext.facingRight ? wjmpSettings.wallJumpForce : -wjmpSettings.wallJumpForce, sharedContext.Rigidbody.velocity.y);
            
            canMove = false;
            sharedContext.MonoBehaviour.StartCoroutine(WaitAndReturnInput(wjmpSettings.noInputTime));
        }

        public override void OnExit()
        {
            base.OnExit();
            
            canMove = false;
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

        public override void SpawnDust()
        {
            Vector2 position = sharedContext.Transform.position;
            Vector2 direction = sharedContext.facingRight ? Vector2.left : Vector2.right;
            
            RaycastHit2D hit = Physics2D.Raycast(position, direction, moveSettings.wallCheckDis, LayerMask.GetMask("Default"));
            
            Debug.DrawLine(position, position + direction * moveSettings.wallCheckDis, Color.red, 0.1f);
            
            if (hit.collider == null)
                return;
            
            if (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform"))
                return;
            
            Transform parent = hit.collider.transform;
            
            Quaternion rotation = Quaternion.Euler(0f, 0f, sharedContext.facingRight ? -90f : 90f);
            
            Object.Instantiate(jumpSettings.dust, hit.point, rotation, parent);
        }
    }
}