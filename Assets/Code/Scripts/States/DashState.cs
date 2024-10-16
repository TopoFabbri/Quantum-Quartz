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
        
        private readonly CameraController camController;

        private bool facingRight;
        private float gravScale;

        public DashState(T id, StateSettings.StateSettings settings, Rigidbody2D rb, MonoBehaviour mb) : base(id,
            settings)
        {
            DashAvailable = true;
            this.rb = rb;
            this.mb = mb;

            if (UnityEngine.Camera.main != null)
                UnityEngine.Camera.main.TryGetComponent(out camController);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            gravScale = rb.gravityScale;
            rb.gravityScale = 0f;

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

        public override void OnFixedUpdate()
        {
            base.OnUpdate();

            rb.velocity = new Vector2((facingRight ? DashSettings.speed : -DashSettings.speed) * Time.fixedDeltaTime,
                0f);
            
            if (WallCheck())
            {
                Ended = true;
                rb.velocity = Vector2.zero;
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

        public bool WallCheck()
        {
            Vector2 pos = (Vector2)rb.gameObject.transform.position +
                          Vector2.right * (DashSettings.wallCheckDis * (facingRight ? 1 : -1));

            Collider2D[] colliders =
                Physics2D.OverlapBoxAll(pos, DashSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Platform") ||
                    collider.gameObject.CompareTag("Floor"))
                    return true;
            }

            return false;
        }
    }
}