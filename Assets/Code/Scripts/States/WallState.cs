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

            sharedContext.Rigidbody.velocity = new Vector2(sharedContext.Rigidbody.velocity.x,
                Mathf.Clamp(sharedContext.Rigidbody.velocity.y, -fallSettings.maxFallSpeed * Time.fixedDeltaTime, fallSettings.maxFallSpeed * Time.fixedDeltaTime));

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
            sharedContext.DoWallCooldown(wallSettings.wallCooldown);
        }

        /// <summary>
        /// Check if player is touching a wall
        /// </summary>
        /// <returns>True if touching a wall</returns>
        public bool CanEnterWall()
        {
            if (ColorSwitcher.Instance.CurrentColor != ColorSwitcher.QColor.Green || sharedContext.inWallCooldown)
                return false;

            return IsTouchingWall(sharedContext.facingRight);
        }

        /// <summary>
        /// Check if player is touching a wall
        /// </summary>
        /// <returns> True if touching a wall</returns>
        public bool IsTouchingWall(bool checkRight)
        {
            return CastCollider(sharedContext.WallStateColliders.UpCollider, checkRight);
        }

        private bool CastCollider(Collider2D collider, bool checkRight)
        {
            List<RaycastHit2D> hits = new();

            collider.enabled = true;
            collider.Cast(Vector2.right, sharedContext.SolidFilter, hits, (checkRight ? sharedContext.GlobalSettings.wallCheckDis : -sharedContext.GlobalSettings.wallCheckDis), true);
            collider.enabled = false;

            foreach (RaycastHit2D hit in hits)
            {
                if (IsWall(hit.transform))
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetHanging(bool checkRight)
        {
            bool midTouching = CastCollider(sharedContext.WallStateColliders.MidCollider, checkRight);
            bool lowTouching = CastCollider(sharedContext.WallStateColliders.LowCollider, checkRight);

            if (!lowTouching)
                return true;

            return !midTouching;
        }

        /// <summary>
        /// Place player at a certain distance from the wall
        /// </summary>
        private void PositionPlayer()
        {
            Vector2 lookDir = sharedContext.facingRight ? Vector2.right : Vector2.left;
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            sharedContext.Collider.Cast(-lookDir, sharedContext.SolidFilter, hits, sharedContext.GlobalSettings.wallCheckDis * 4f, true);

            hits = hits.Where((hit) => IsWall(hit.transform)).ToList();

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


        private bool IsWall(Transform t)
        {
            return t && (t.CompareTag("Wall") || t.CompareTag("Floor") || (t.CompareTag("Platform") && !t.TryGetComponent(out PlatformEffector2D _)));
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
            RaycastHit2D hit = Physics2D.Raycast((Vector2)sharedContext.Transform.position + facingDir, -facingDir, wallSettings.wallDis, sharedContext.SolidFilter.layerMask);

            if (!hit.collider)
                return;

            Vector2 position = (Vector2)sharedContext.Transform.position + facingDir * wallSettings.wallDis + Vector2.down * wallSettings.dustOffset;
            Quaternion rotation = Quaternion.Euler(0f, 0f, sharedContext.facingRight ? 90f : -90f);

            Object.Instantiate(wallSettings.dust, position, rotation);
        }
    }
}