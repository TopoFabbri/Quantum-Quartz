using Code.Scripts.FSM;
using Code.Scripts.Game.Managers;
using Code.Scripts.Player;
using Code.Scripts.States.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Manage player dash state
    /// </summary>
    /// <typeparam name="T">Id</typeparam>
    public class DashState<T> : BaseState<T>, INoCoyoteTime, IUnsafe
    {
        protected readonly DashSettings dashSettings;

        public bool Ended { get; private set; }

        private readonly SharedContext sharedContext;
        private readonly BarController barController;
        private readonly ParticleSystem dashParticleSystem;

        private float gravScale;
        private bool interrupted;
        private Coroutine coroutine;

        public DashState(T id, DashSettings stateSettings, SharedContext sharedContext, BarController barController, ParticleSystem dashParticleSystem) : base(id)
        {
            this.dashSettings = stateSettings;
            this.sharedContext = sharedContext;
            this.barController = barController;
            this.dashParticleSystem = dashParticleSystem;
            
            barController.AddBar(ColorSwitcher.QColor.Red, dashSettings.staminaRegenSpeed, dashSettings.staminaMitigationAmount, 0f);
            barController.GetBar(ColorSwitcher.QColor.Red).AddConditionalRegenSpeed(() => sharedContext.IsGrounded ? dashSettings.staminaFloorRegenSpeed : null);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            dashParticleSystem.Play();

            interrupted = true;
            Ended = false;
            gravScale = sharedContext.Rigidbody.gravityScale;
            sharedContext.Rigidbody.gravityScale = 0f;
            sharedContext.Rigidbody.velocity = Vector2.zero;
            sharedContext.SetFalling(false);
            sharedContext.BlockMoveInput = false; //Workaround, to get CheckFlip to run and ensure you dash in the right direction
            sharedContext.BlockMoveInput = true;

            sharedContext.CamController?.Shake(dashSettings.shakeDur, dashSettings.shakeMag);

            coroutine = sharedContext.MonoBehaviour.StartCoroutine(EndDash());
            barController.GetBar(ColorSwitcher.QColor.Red).Use();
        }

        public override void OnExit()
        {
            base.OnExit();

            if (!interrupted)
                sharedContext.Rigidbody.velocity = new Vector2(sharedContext.Rigidbody.velocity.x / 2f, sharedContext.Rigidbody.velocity.y);

            sharedContext.Rigidbody.gravityScale = gravScale;
            sharedContext.SetFalling(true);
            sharedContext.BlockMoveInput = false;



            if (coroutine != null)
            {
                sharedContext.MonoBehaviour.StopCoroutine(coroutine);
            }
            barController.GetBar(ColorSwitcher.QColor.Red).Use();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            barController.GetBar(ColorSwitcher.QColor.Red).Use();

            if (WallCheck())
            {
                interrupted = false;
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
            yield return new WaitForSeconds(dashSettings.duration);
            Ended = true;
        }

        private bool WallCheck()
        {
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            sharedContext.Collider.Cast(sharedContext.facingRight ? Vector2.right : Vector2.left, sharedContext.SolidFilter, hits, dashSettings.wallCheckDis, true);

            foreach (RaycastHit2D hit in hits)
            {
                if (dashSettings.tags.Any(tag => hit.transform.CompareTag(tag)))
                    return true;
            }

            return false;
        }
    }
}