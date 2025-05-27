using System.Collections;
using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Platforms;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;
using System.Linq;

namespace Code.Scripts.States
{
    public class WallState<T> : FallState<T>
    {
        protected readonly WallSettings wallSettings;

        private float savedGravityScale;

        public WallState(T id, WallSettings stateSettings, SharedContext sharedContext) : base(id, stateSettings.fallSettings, sharedContext)
        {
            wallSettings = stateSettings;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            sharedContext.Rigidbody.gravityScale = sharedContext.Rigidbody.velocity.y < 0 ? wallSettings.gravMultiplier : wallSettings.upwardsGravMultiplier;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            sharedContext.Rigidbody.velocity = new Vector2(
                sharedContext.Rigidbody.velocity.x,
                Mathf.Clamp(sharedContext.Rigidbody.velocity.y, -fallSettings.maxFallSpeed * Time.fixedDeltaTime, fallSettings.maxFallSpeed * Time.fixedDeltaTime)
            );

            PositionPlayer();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            savedGravityScale = sharedContext.Rigidbody.gravityScale;
            
            PositionPlayer();

            sharedContext.MonoBehaviour.StartCoroutine(SpawnDusts());
        }

        public override void OnExit()
        {
            base.OnExit();

            sharedContext.Transform.parent = null;

            sharedContext.Rigidbody.gravityScale = savedGravityScale;
        }

        /// <summary>
        /// Check if player is touching a wall
        /// </summary>
        /// <returns>True if touching a wall</returns>
        public bool CanEnterWall()
        {
            if (ColorSwitcher.Instance.CurrentColour != ColorSwitcher.QColour.Green)
                return false;

            return IsTouchingWall(sharedContext.facingRight);
        }

        /// <summary>
        /// Check if player is touching a wall
        /// </summary>
        /// <returns> True if touching a wall</returns>
        public bool IsTouchingWall(bool checkRight)
        {
            Vector2 pos = (Vector2)sharedContext.Transform.position + Vector2.right * (checkRight ? moveSettings.wallCheckDis : -moveSettings.wallCheckDis);

            Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, moveSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Floor"))
                    return true;

                if (!collider.gameObject.CompareTag("Platform"))
                    continue;

                if (collider.TryGetComponent(out PlatformEffector2D platformEffector2D))
                    continue;
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Place player at a certain distance from the wall
        /// </summary>
        private void PositionPlayer()
        {
            Vector2 lookDir = sharedContext.facingRight ? Vector3.right : Vector3.left;
            Vector2 checkOrigin = (Vector2)sharedContext.Transform.position;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(checkOrigin, moveSettings.wallCheckSize, 0f, -lookDir, moveSettings.wallCheckDis * 4f, LayerMask.GetMask("Default"));

            hits = hits.Where((hit) => hit.collider && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform"))).ToArray();

            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.TryGetComponent(out ObjMovement objMovement))
                    {
                        objMovement.AddPlayer(sharedContext.Transform);
                        break;
                    }
                }

                Vector3 newPos = sharedContext.Transform.position;
                newPos.x = hits[0].point.x + (sharedContext.facingRight ? 1f : -1f) * wallSettings.wallDis;

                sharedContext.Transform.position = newPos;
            }
        }

        /// <summary>
        /// Spawn wall dusts
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnDusts()
        {
            for (int i = 0; i < wallSettings.dustQty; i++)
            {
                SpawnDust();
                yield return new WaitForSeconds(wallSettings.dustDelay);
            }
        }

        /// <summary>
        /// Spawn wall dust
        /// </summary>
        private void SpawnDust()
        {
            RaycastHit2D hit = Physics2D.Raycast(sharedContext.Transform.position + (sharedContext.facingRight ? Vector3.right : Vector3.left),
                sharedContext.facingRight ? Vector2.left : Vector2.right, wallSettings.wallDis,
                LayerMask.GetMask("Default"));

            if (!hit.collider)
                return;
            
            Vector2 position = sharedContext.Transform.position +
                               (sharedContext.facingRight ? Vector3.right : Vector3.left) * wallSettings.wallDis +
                               Vector3.down * wallSettings.dustOffset;
            Quaternion rotation = Quaternion.Euler(0f, 0f, sharedContext.facingRight ? 90f : -90f);

            Object.Instantiate(wallSettings.dust, position, rotation);
        }
    }
}