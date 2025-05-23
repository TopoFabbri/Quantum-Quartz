using System.Collections;
using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Platforms;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class WallState<T> : FallState<T>
    {
        protected readonly WallSettings wallSettings;

        private float savedGravityScale;

        public WallState(T id, WallSettings stateSettings, PlayerState.SharedContext sharedContext) : base(id, stateSettings.fallSettings, sharedContext)
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

            Vector2 pos = (Vector2)sharedContext.Transform.position + Vector2.right * (sharedContext.facingRight ? moveSettings.wallCheckDis : -moveSettings.wallCheckDis);

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
        /// Check if player is touching a wall
        /// </summary>
        /// <returns> True if touching a wall</returns>
        public bool IsTouchingWall()
        {
            Vector2 pos = (Vector2)sharedContext.Transform.position +
                          Vector2.right * (sharedContext.facingRight ? -moveSettings.wallCheckDis : moveSettings.wallCheckDis);

            Collider2D[] colliders =
                Physics2D.OverlapBoxAll(pos, moveSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Floor"))
                    return true;

                if (!collider.gameObject.CompareTag("Platform")) continue;

                if (collider.TryGetComponent(out PlatformEffector2D platformEffector2D)) continue;
                
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Place player at a certain distance from the wall
        /// </summary>
        private void PositionPlayer()
        {
            RaycastHit2D hit = Physics2D.Raycast(sharedContext.Transform.position + (sharedContext.facingRight ? Vector3.right : Vector3.left),
                sharedContext.facingRight ? Vector2.left : Vector2.right, moveSettings.wallCheckDis * 4f,
                LayerMask.GetMask("Default"));
            
            if (!hit.collider || !hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform"))
                return;

            if (hit.collider.TryGetComponent(out ObjMovement objMovement))
                objMovement.AddPlayer(sharedContext.Transform);
            
            Vector3 newPos = sharedContext.Transform.position;
            newPos.x = hit.point.x + (sharedContext.facingRight ? 1f : -1f) * wallSettings.wallDis;

            sharedContext.Transform.position = newPos;
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