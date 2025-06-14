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

        private float impulse = 0;
        private Coroutine coroutine = null;

        public WallJumpState(T id, WjmpSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.jumpSettings, sharedContext)
        {
            this.wjmpSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            impulse = sharedContext.facingRight ? wjmpSettings.wallJumpForce : -wjmpSettings.wallJumpForce;
            sharedContext.speed.x = impulse;
            sharedContext.BlockMoveInput = true;
            coroutine = sharedContext.MonoBehaviour.StartCoroutine(WaitAndReturnInput(wjmpSettings.noInputTime));
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.speed.x = 0;
            sharedContext.BlockMoveInput = false;

            if (coroutine != null)
            {
                sharedContext.MonoBehaviour.StopCoroutine(coroutine);
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (!sharedContext.Falling)
            {
                sharedContext.speed.x = impulse + sharedContext.speed.x;
                sharedContext.Rigidbody.velocity = sharedContext.speed;
            }
        }

        /// <summary>
        /// Stop input for given time
        /// </summary>
        /// <param name="noInputTime">Time to wait</param>
        /// <returns></returns>
        private IEnumerator WaitAndReturnInput(float noInputTime)
        {
            yield return new WaitForSeconds(noInputTime);
            sharedContext.BlockMoveInput = false;
        }

        public override void SpawnDust()
        {
            Vector2 position = sharedContext.Transform.position;
            Vector2 direction = sharedContext.facingRight ? Vector2.left : Vector2.right;
            
            RaycastHit2D hit = Physics2D.Raycast(position, direction, moveSettings.wallCheckDis, LayerMask.GetMask("Default"));
            
            Debug.DrawLine(position, position + direction * moveSettings.wallCheckDis, Color.red, 0.1f);
            
            if (hit.collider == null || (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform")))
                return;
            
            Transform parent = hit.collider.transform;
            
            Quaternion rotation = Quaternion.Euler(0f, 0f, sharedContext.facingRight ? -90f : 90f);
            
            Object.Instantiate(jumpSettings.dust, hit.point, rotation, parent);
        }
    }
}