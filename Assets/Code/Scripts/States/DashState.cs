using System.Collections;
using System.Collections.Generic;
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
    public class DashState<T> : BaseState<T>, INoCoyoteTime, IUnsafe
    {
        protected readonly DashSettings dashSettings;

        public bool Ended { get; private set; }

        private readonly SharedContext sharedContext;
        private readonly BarController barController;
        private readonly ParticleSystem dashParticleSystem;
        private static ContactFilter2D contactFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Default")
        };

        private float gravScale;
        private bool interrupted;

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

            interrupted = false;
            gravScale = sharedContext.Rigidbody.gravityScale;
            sharedContext.Rigidbody.gravityScale = 0f;
            sharedContext.Rigidbody.velocity = Vector2.zero;
            sharedContext.SetFalling(false);
            sharedContext.BlockMoveInput = false; //Workaround, to get CheckFlip to run and ensure you dash in the right direction
            sharedContext.BlockMoveInput = true;

            sharedContext.CamController?.Shake(dashSettings.shakeDur, dashSettings.shakeMag);

            sharedContext.MonoBehaviour.StartCoroutine(EndDash());
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

            barController.GetBar(ColorSwitcher.QColor.Red).Use();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            barController.GetBar(ColorSwitcher.QColor.Red).Use();

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
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            sharedContext.Collider.Cast(sharedContext.facingRight ? Vector2.right : Vector2.left, contactFilter, hits, dashSettings.wallCheckDis, true);

            foreach (RaycastHit2D hit in hits)
            {
                if (dashSettings.tags.Any(tag => hit.transform.CompareTag(tag)))
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