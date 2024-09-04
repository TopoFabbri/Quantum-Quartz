using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class WallState<T> : FallState<T>
    {
        private WallSettings WallSettings => settings as WallSettings;

        private float savedGravityScale;
        
        public WallState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx) : base(id, stateSettings, rb, transform, mb, playerSfx)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            savedGravityScale = rb.gravityScale;
            rb.gravityScale = WallSettings.gravMultiplier;
        }

        public override void OnExit()
        {
            base.OnExit();

            rb.gravityScale = savedGravityScale;
        }

        public bool IsAgainstWall()
        {
            Vector2 pos = (Vector2)transform.position + Vector2.right * (WallSettings.wallCheckDis * Input);
            
            Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, moveSettings.wallCheckSize, 0, LayerMask.GetMask("Default"));

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Floor"))
                    return true;

                if (!collider.gameObject.CompareTag("Platform")) continue;
                
                if (!collider.TryGetComponent(out PlatformEffector2D platformEffector2D))
                    return true;
            }
            
            return false;
        }
    }
}