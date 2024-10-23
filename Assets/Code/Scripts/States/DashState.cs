using System.Collections;
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

        public DashState(T id, StateSettings.StateSettings settings, Rigidbody2D rb, Transform transform,
            MonoBehaviour mb) : base(id,
            settings)
        {
            DashAvailable = true;
            this.rb = rb;
            this.mb = mb;
            this.transform = transform;

            if (UnityEngine.Camera.main != null)
                UnityEngine.Camera.main.TryGetComponent(out camController);
        }

        public override void OnEnter()
        {
            base.OnEnter();

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

            rb.velocity = new Vector2(rb.velocity.x / 2f, 0f);
            rb.gravityScale = gravScale;

            mb.StartCoroutine(StartCoolDown());
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            transform.position = Vector3.MoveTowards(transform.position,
                transform.position + Vector3.right * (facingRight ? 1 : -1), DashSettings.speed * Time.deltaTime);

            if (WallCheck())
                Ended = true;
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

        public bool WallCheck()
        {
            Vector2 pos = (Vector2)rb.gameObject.transform.position +
                          Vector2.right * (DashSettings.wallCheckDis * (facingRight ? 1 : -1));

            Collider2D[] colliders =
                Physics2D.OverlapBoxAll(pos, DashSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Floor"))
                    return true;
            }

            return false;
        }
    }
}