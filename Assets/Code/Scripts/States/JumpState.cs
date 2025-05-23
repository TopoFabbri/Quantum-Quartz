using System.Collections;
using Code.Scripts.FSM;
using Code.Scripts.Player;
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
        protected readonly JumpSettings jumpSettings;

        public bool HasJumped { get; protected set; }
        public float JumpForce => jumpSettings.jumpForce;
        
        public JumpState(T id, JumpSettings stateSettings, PlayerState.SharedContext sharedContext) : base(id, stateSettings.moveSettings, sharedContext)
        {
            this.jumpSettings = stateSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sharedContext.PlayerSfx.Jump();

            if (sharedContext.PreviousStateType == typeof(FallState<T>))
            {
                Vector2 vector2 = sharedContext.Rigidbody.velocity;
                vector2.y = 0f;
                sharedContext.Rigidbody.velocity = vector2;
            }


            sharedContext.MonoBehaviour.StartCoroutine(JumpOnFU());

            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.airFriction;
            
            SpawnDust();
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.Rigidbody.sharedMaterial.friction = moveSettings.groundFriction;
            
            HasJumped = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!sharedContext.IsGrounded())
                HasJumped = true;
        }

        /// <summary>
        /// Wait for fixed update and jump
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator JumpOnFU()
        {
            yield return new WaitForFixedUpdate();

            sharedContext.Rigidbody.AddForce(jumpSettings.jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Make dust at jump position
        /// </summary>
        public virtual void SpawnDust()
        {
            Vector2 position = (Vector2)sharedContext.Transform.position + sharedContext.GlobalSettings.groundCheckOffset;
            
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, sharedContext.GlobalSettings.groundCheckRadius, LayerMask.GetMask("Default"));
            
            if (hit.collider == null)
                return;
            
            if (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform"))
                return;
            
            Transform parent = hit.collider.transform;
            
            Object.Instantiate(jumpSettings.dust, hit.point, Quaternion.identity, parent);
        }
    }
}