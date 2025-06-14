using System.Collections;
using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Platforms;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Code.Scripts.States
{
    public class WallState<T> : FallState<T>, IPreventFlip
    {
        protected readonly WallSettings wallSettings;

        private float savedGravityScale;
        private static ContactFilter2D contactFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Default")
        };

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
            sharedContext.jumpFallTime = 0;
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
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            sharedContext.Collider.Cast(Vector2.right, contactFilter, hits, (checkRight ? moveSettings.wallCheckDis : -moveSettings.wallCheckDis), true);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Floor"))
                    return true;

                if (!hit.transform.CompareTag("Platform") || hit.transform.TryGetComponent(out PlatformEffector2D platformEffector2D))
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
            Vector2 lookDir = sharedContext.facingRight ? Vector2.right : Vector2.left;
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            sharedContext.Collider.Cast(-lookDir, contactFilter, hits, moveSettings.wallCheckDis * 4f, true);

            hits = hits.Where((hit) => hit.transform && (hit.transform.CompareTag("Floor") || hit.transform.CompareTag("Platform"))).ToList();

            if (hits.Count > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.transform.TryGetComponent(out ObjectMovement objMovement))
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
            Vector2 facingDir = sharedContext.facingRight ? Vector3.right : Vector3.left;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)sharedContext.Transform.position + facingDir, -facingDir, wallSettings.wallDis, LayerMask.GetMask("Default"));

            if (!hit.collider)
                return;
            
            Vector2 position = (Vector2)sharedContext.Transform.position + facingDir * wallSettings.wallDis + Vector2.down * wallSettings.dustOffset;
            Quaternion rotation = Quaternion.Euler(0f, 0f, sharedContext.facingRight ? 90f : -90f);

            Object.Instantiate(wallSettings.dust, position, rotation);
        }
    }
}