using System.Collections;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Falling state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FallState<T> : MoveState<T>
    {
        private FallSettings FallSettings => settings as FallSettings;
        
        public FallState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx) : base(id, stateSettings, rb, transform)
        {
            settings = stateSettings;
            moveSettings = FallSettings.moveSettings;

            this.mb = mb;
            this.playerSfx = playerSfx;
        }

        private readonly MonoBehaviour mb;
        private readonly PlayerSfx playerSfx;
        public bool CanCoyoteJump { get; private set; }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            rb.sharedMaterial.friction = FallSettings.moveSettings.airFriction;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            rb.sharedMaterial.friction = FallSettings.moveSettings.groundFriction;
            
            if (IsGrounded())
                playerSfx.Land();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input != 0)
                rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        public void StartCoyoteTime()
        {
            CanCoyoteJump = true;
            mb.StartCoroutine(StopCoyoteTime());
        }
        
        private IEnumerator StopCoyoteTime()
        {
            yield return new WaitForSeconds(FallSettings.coyoteTime);
            CanCoyoteJump = false;
        }
    }
}