using System.Collections;
using System.Linq;
using Code.Scripts.Camera;
using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Manage player dash state
    /// </summary>
    /// <typeparam name="T">Id</typeparam>
    public class DashState<T> : BaseState<T>, INoCoyoteTime
    {
        protected readonly DashSettings dashSettings;

        public bool Ended { get; private set; }

        private readonly SharedContext sharedContext;
        private readonly BarController barController;
        private readonly ParticleSystem dashParticleSystem;

        private float gravScale;
        private bool interrupted;

        public DashState(T id, DashSettings stateSettings, SharedContext sharedContext, BarController barController, ParticleSystem dashParticleSystem) : base(id)
        {
            this.dashSettings = stateSettings;
            this.sharedContext = sharedContext;
            this.barController = barController;
            this.dashParticleSystem = dashParticleSystem;
            
            barController.AddBar(ColorSwitcher.QColour.Red, dashSettings.staminaRegenSpeed, dashSettings.staminaMitigationAmount, 0f);
            barController.GetBar(ColorSwitcher.QColour.Red).AddConditionalRegenSpeed(() => sharedContext.IsGrounded ? dashSettings.staminaFloorRegenSpeed : null);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            dashParticleSystem.Play();

            interrupted = false;
            gravScale = sharedContext.Rigidbody.gravityScale;
            sharedContext.Rigidbody.gravityScale = 0f;
            sharedContext.Rigidbody.velocity = Vector2.zero;

            sharedContext.CamController?.Shake(dashSettings.shakeDur, dashSettings.shakeMag);

            sharedContext.MonoBehaviour.StartCoroutine(EndDash());
            barController.GetBar(ColorSwitcher.QColour.Red).Use();
        }

        public override void OnExit()
        {
            base.OnExit();

            if (!interrupted)
                sharedContext.Rigidbody.velocity = new Vector2(sharedContext.Rigidbody.velocity.x / 2f, sharedContext.Rigidbody.velocity.y);

            sharedContext.Rigidbody.gravityScale = gravScale;
            
            barController.GetBar(ColorSwitcher.QColour.Red).Use();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            barController.GetBar(ColorSwitcher.QColour.Red).Use();

            if (WallCheck())
            {
                Ended = true;
            }
            else
            {
                sharedContext.Transform.position = Vector3.MoveTowards(
                    sharedContext.Transform.position,
                    sharedContext.Transform.position + Vector3.right * (sharedContext.facingRight ? 1 : -1),
                    dashSettings.speed * Time.fixedDeltaTime
                );
            }
        }

        /// <summary>
        /// Wait dash duration and set as ended
        /// </summary>
        /// <returns></returns>
        private IEnumerator EndDash()
        {
            Ended = false;

            yield return new WaitForSeconds(dashSettings.duration);
            Ended = true;
        }

        private bool WallCheck()
        {
            Vector2 pos = (Vector2)sharedContext.Rigidbody.gameObject.transform.position + Vector2.right * (dashSettings.wallCheckDis * (sharedContext.facingRight ? 1 : -1));

            Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, dashSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (dashSettings.tags.Any(tag => collider.gameObject.CompareTag(tag)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Interrupt the dash state. This sets the state's Ended property to true.
        /// </summary>
        public void Interrupt()
        {
            interrupted = true;
            Ended = true;
        }
    }
}