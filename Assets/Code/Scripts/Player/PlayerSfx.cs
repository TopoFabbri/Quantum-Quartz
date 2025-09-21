using Code.Scripts.Game.Obstacles;
using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage
    /// </summary>
    public class PlayerSfx : MonoBehaviour
    {
        [SerializeField] private Event stepEvent;
        [SerializeField] private Event jumpEvent;
        [SerializeField] private Event landEvent;
        [SerializeField] private Event djmpEvent;
        [SerializeField] private Event dashEvent;
        [SerializeField] private Event glideStartEvent;
        [SerializeField] private Event glideStopEvent;
        [SerializeField] private Event deathEvent;
        [SerializeField] private Event floorSwitchEvent;

        private ContactFilter2D solidFilter = new ContactFilter2D();

        private void Start()
        {
            solidFilter = new ContactFilter2D
            {
                layerMask = LayerMask.GetMask("Default", "SolidTiles")
            };
        }

        /// <summary>
        /// Call step event
        /// </summary>
        public void Step()
        {
            CheckMaterial();
            stepEvent.Post(gameObject);
        }
        
        /// <summary>
        /// Call jump event
        /// </summary>
        public void Jump()
        {
            jumpEvent.Post(gameObject);
        }
        
        /// <summary>
        /// Call land event
        /// </summary>
        public void Land()
        {
            CheckMaterial();
            landEvent.Post(gameObject);
        }
        
        /// <summary>
        /// Call djmp event
        /// </summary>
        public void Djmp()
        {
            djmpEvent.Post(gameObject);
        }

        /// <summary>
        /// Call left dash event
        /// </summary>
        public void DashL()
        {
            dashEvent.Post(gameObject);
        }
        
        /// <summary>
        /// Call right dash event
        /// </summary>
        public void DashR()
        {
            dashEvent.Post(gameObject);
        }

        /// <summary>
        /// Call glide event
        /// </summary>
        public void PlayGlide()
        {
            glideStartEvent.Post(gameObject);
        }

        /// <summary>
        /// Call stop glide event
        /// </summary>
        public void StopGlide()
        {
            glideStopEvent.Post(gameObject);
        }
        
        /// <summary>
        /// Call death event
        /// </summary>
        public void Death()
        {
            deathEvent.Post(gameObject);
        }

        /// <summary>
        /// Set the floor material sound
        /// </summary>
        private void CheckMaterial()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, solidFilter.layerMask);

            if (!hit.collider) return;
            
            GameObject other = hit.collider.gameObject;

            if (other.TryGetComponent(out PlatformController plat))
            {
                plat.matSoundEvent.Post(gameObject);
            }
            else
            {
                floorSwitchEvent.Post(gameObject);
            }
        }
    }
}