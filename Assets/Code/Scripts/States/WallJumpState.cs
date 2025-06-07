using System.Collections;
using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class WallJumpState<T> : JumpState<T>, IPreventFlip
    {
        protected readonly WjmpSettings wjmpSettings;

        private float impulse = 0;
        private float lastVel = 0;

        public WallJumpState(T id, WjmpSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.jumpSettings, sharedContext)
        {
            this.wjmpSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            impulse = sharedContext.facingRight ? wjmpSettings.wallJumpForce : -wjmpSettings.wallJumpForce;
            sharedContext.speed.x = impulse;
            sharedContext.blockMoveInput = true;
            sharedContext.MonoBehaviour.StartCoroutine(WaitAndReturnInput(wjmpSettings.noInputTime));
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.speed.x = 0;
            sharedContext.blockMoveInput = false;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (!sharedContext.Falling)
            {
                sharedContext.speed.x = sharedContext.facingRight ? wjmpSettings.wallJumpForce : -wjmpSettings.wallJumpForce;

                sharedContext.jumpFallTime += Time.fixedDeltaTime;
                float vel = impulse * verticalVelocityCurve.SampleVelocity(sharedContext.jumpFallTime) / verticalVelocityCurve.HeightScale;
                sharedContext.speed.x = (vel + lastVel) * 0.5f + sharedContext.speed.x;
                sharedContext.Rigidbody.velocity = sharedContext.speed;
                lastVel = vel;
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
            sharedContext.blockMoveInput = false;
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