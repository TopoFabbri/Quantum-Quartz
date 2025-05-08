using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Code.Scripts.Camera;
using Code.Scripts.FSM;
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
        private DashSettings DashSettings => settings as DashSettings;

        public bool Ended { get; private set; }
        public bool DashAvailable { get; private set; }

        private readonly Rigidbody2D rb;
        private readonly MonoBehaviour mb;
        private readonly Transform transform;

        private readonly CameraController camController;

        private bool facingRight;
        private float gravScale;
        private bool interrupted;

        public DashState(T id, StateSettings.StateSettings settings, Rigidbody2D rb, Transform transform,
            MonoBehaviour mb) : base(id,
            settings)
        {
            DashAvailable = true;
            this.rb = rb;
            this.mb = mb;
            this.transform = transform;

            UnityEngine.Camera.main?.transform.parent?.TryGetComponent(out camController);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            interrupted = false;
            gravScale = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;

            if (camController)
                camController.Shake(DashSettings.shakeDur, DashSettings.shakeMag);

            mb.StartCoroutine(EndDash());
        }

        public override void OnExit()
        {
            base.OnExit();

            if (!interrupted)
                rb.velocity = new Vector2(rb.velocity.x / 2f, rb.velocity.y);
            
            rb.gravityScale = gravScale;

            mb.StartCoroutine(StartCoolDown());
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
                    DashSettings.speed * Time.fixedDeltaTime
                );
            }
        }

        /// <summary>
        /// Wait dash duration and set as ended
        /// </summary>
        /// <returns></returns>
        private IEnumerator EndDash()
        {
            DashAvailable = false;
            Ended = false;

            yield return new WaitForSeconds(DashSettings.duration);
            Ended = true;
        }

        /// <summary>
        /// Wait cooldown and reset
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartCoolDown()
        {
            yield return new WaitForSeconds(DashSettings.cooldown);

            Reset();
        }

        /// <summary>
        /// Reset dash
        /// </summary>
        public void Reset()
        {
            DashAvailable = true;
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
            Vector2 pos = (Vector2)rb.gameObject.transform.position +
                          Vector2.right * (DashSettings.wallCheckDis * (facingRight ? 1 : -1));

            Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, DashSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (DashSettings.tags.Any(tag => collider.gameObject.CompareTag(tag)))
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