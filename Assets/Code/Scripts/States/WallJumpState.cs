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
        private float noInputTime = 0;

        public WallJumpState(T id, WjmpSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.jumpSettings, sharedContext)
        {
            this.wjmpSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            impulse = sharedContext.facingRight ? wjmpSettings.wallJumpForce : -wjmpSettings.wallJumpForce;
            sharedContext.SpeedX = impulse;
            sharedContext.BlockMoveInput = true;
            noInputTime = wjmpSettings.noInputTime;
            sharedContext.DoWallCooldown(wjmpSettings.wallCooldown);
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.SpeedX = 0;
            sharedContext.BlockMoveInput = false;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (!sharedContext.Falling)
            {
                sharedContext.SpeedX = impulse + sharedContext.Speed.x;
                sharedContext.Rigidbody.velocity = sharedContext.Speed;
            }
            noInputTime -= Time.fixedDeltaTime;
            if (noInputTime <= 0)
            {
                sharedContext.BlockMoveInput = false;
            }
        }

        public override void SpawnDust()
        {
            Vector2 position = sharedContext.Transform.position;
            Vector2 direction = sharedContext.facingRight ? Vector2.left : Vector2.right;
            
            RaycastHit2D hit = Physics2D.Raycast(position, direction, sharedContext.GlobalSettings.wallCheckDis, LayerMask.GetMask("Default"));
            
            Debug.DrawLine(position, position + direction * sharedContext.GlobalSettings.wallCheckDis, Color.red, 0.1f);
            
            if (hit.collider == null || (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform")))
                return;
            
            Transform parent = hit.collider.transform;
            
            Quaternion rotation = Quaternion.Euler(0f, 0f, sharedContext.facingRight ? -90f : 90f);
            
            Object.Instantiate(jumpSettings.dust, hit.point, rotation, parent);
        }
    }
}