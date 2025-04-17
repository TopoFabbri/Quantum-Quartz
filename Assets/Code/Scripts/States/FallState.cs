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

        protected readonly MonoBehaviour mb;
        private readonly PlayerSfx playerSfx;
        public bool CanCoyoteJump { get; private set; }
        
        public FallState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx) : base(id, stateSettings, rb, transform)
        {
            settings = stateSettings;

            this.mb = mb;
            this.playerSfx = playerSfx;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            if (IsGrounded())
                playerSfx.Land();
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