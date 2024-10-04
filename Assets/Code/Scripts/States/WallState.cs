using System.Collections;
using Code.Scripts.Colors;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class WallState<T> : FallState<T>
    {
        private WallSettings WallSettings => settings as WallSettings;

        private float savedGravityScale;

        public bool FacingRight { get; set; }

        public WallState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform,
            MonoBehaviour mb, PlayerSfx playerSfx) : base(id, stateSettings, rb, transform, mb, playerSfx)
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            rb.gravityScale = rb.velocity.y < 0 ? WallSettings.gravMultiplier : WallSettings.upwardsGravMultiplier;
            PositionPlayer();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            rb.velocity = new Vector2(rb.velocity.x,
                Mathf.Clamp(rb.velocity.y, -WallSettings.maxFallSpeed * Time.fixedDeltaTime,
                    WallSettings.maxFallSpeed * Time.fixedDeltaTime));
        }

        public override void OnEnter()
        {
            base.OnEnter();

            savedGravityScale = rb.gravityScale;

            PositionPlayer();
            mb.StartCoroutine(SpawnDusts());
        }

        public override void OnExit()
        {
            base.OnExit();

            rb.gravityScale = savedGravityScale;
        }

        /// <summary>
        /// Check if player is touching a wall
        /// </summary>
        /// <returns>True if touching a wall</returns>
        public bool CanWallJump()
        {
            if ((FacingRight && Input <= 0f) || (!FacingRight && Input >= 0f)) return false;
            
            if (ColorSwitcher.Instance.CurrentColor != ColorSwitcher.QColor.Green)
                return false;

            Vector2 pos = (Vector2)transform.position +
                          Vector2.right * (FacingRight ? WallSettings.wallCheckDis : -WallSettings.wallCheckDis);

            Collider2D[] colliders =
                Physics2D.OverlapBoxAll(pos, moveSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Floor"))
                    return true;

                if (!collider.gameObject.CompareTag("Platform")) continue;

                if (!collider.TryGetComponent(out PlatformEffector2D _))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Place player at a certain distance from the wall
        /// </summary>
        private void PositionPlayer()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + (FacingRight ? Vector3.left : Vector3.right),
                FacingRight ? Vector2.right : Vector2.left, WallSettings.wallCheckDis * 4f,
                LayerMask.GetMask("Default"));

            if (!hit.collider)
                return;

            Vector3 newPos = transform.position;
            newPos.x = hit.point.x + (FacingRight ? -1f : 1f) * WallSettings.wallDis;

            transform.position = newPos;
        }

        /// <summary>
        /// Spawn wall dusts
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnDusts()
        {
            for (int i = 0; i < WallSettings.dustQty; i++)
            {
                SpawnDust();
                yield return new WaitForSeconds(WallSettings.dustDelay);
            }
        }

        /// <summary>
        /// Spawn wall dust
        /// </summary>
        private void SpawnDust()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + (FacingRight ? Vector3.right : Vector3.left),
                FacingRight ? Vector2.left : Vector2.right, WallSettings.wallDis,
                LayerMask.GetMask("Default"));

            if (!hit.collider)
                return;
            
            Vector2 position = transform.position +
                               (FacingRight ? Vector3.right : Vector3.left) * WallSettings.wallDis +
                               Vector3.down * WallSettings.dustOffset;
            Quaternion rotation = Quaternion.Euler(0f, 0f, FacingRight ? 90f : -90f);

            Object.Instantiate(WallSettings.dust, position, rotation);
        }
    }
}