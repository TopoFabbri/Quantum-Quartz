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
    public class DashState<T> : BaseState<T>
    {
        protected readonly DashSettings dashSettings;

        public bool Ended { get; private set; }

        private readonly Rigidbody2D rb;
        private readonly MonoBehaviour mb;
        private readonly Transform transform;
        private readonly BarController barController;

        private readonly CameraController camController;

        private bool facingRight;
        private float gravScale;
        private bool interrupted;

        public DashState(T id, DashSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, BarController barController) : base(id)
        {
            this.dashSettings = stateSettings;
            this.rb = rb;
            this.mb = mb;
            this.transform = transform;
            this.barController = barController;

            UnityEngine.Camera.main?.transform.parent?.TryGetComponent(out camController);
            
            barController.AddBar(ColorSwitcher.QColour.Red, dashSettings.staminaRegenSpeed, dashSettings.staminaMitigationAmount, 0f);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            interrupted = false;
            gravScale = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;

            if (camController)
                camController.Shake(dashSettings.shakeDur, dashSettings.shakeMag);

            mb.StartCoroutine(EndDash());
        }

        public override void OnExit()
        {
            base.OnExit();

            if (!interrupted)
                rb.velocity = new Vector2(rb.velocity.x / 2f, rb.velocity.y);
            
            rb.gravityScale = gravScale;
            
            barController.GetBar(ColorSwitcher.QColour.Red).Use();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            
            if (WallCheck())
            {
                Ended = true;
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    transform.position + Vector3.right * (facingRight ? 1 : -1),
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

        /// <summary>
        /// Flip dash direction
        /// </summary>
        public void Flip()
        {
            facingRight = !facingRight;
        }

        private bool WallCheck()
        {
            Vector2 pos = (Vector2)rb.gameObject.transform.position + Vector2.right * (dashSettings.wallCheckDis * (facingRight ? 1 : -1));

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