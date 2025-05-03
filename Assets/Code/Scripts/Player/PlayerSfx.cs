using System;
using Code.Scripts.Platforms;
using UnityEngine;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage
    /// </summary>
    public class PlayerSfx : MonoBehaviour
    {
        [SerializeField] private string stepEvent;
        [SerializeField] private string jumpEvent;
        [SerializeField] private string landEvent;
        [SerializeField] private string djmpEvent;
        [SerializeField] private string floorSwitchEvent = "Set_Switch_Concrete";

        /// <summary>
        /// Call step event
        /// </summary>
        public void Step()
        {
            CheckMaterial();
            AkSoundEngine.PostEvent(stepEvent, gameObject);
        }
        
        /// <summary>
        /// Call jump event
        /// </summary>
        public void Jump()
        {
            AkSoundEngine.PostEvent(jumpEvent, gameObject);
        }
        
        /// <summary>
        /// Call land event
        /// </summary>
        public void Land()
        {
            CheckMaterial();
            AkSoundEngine.PostEvent(landEvent, gameObject);
        }
        
        /// <summary>
        /// Call djmp event
        /// </summary>
        public void Djmp()
        {
            AkSoundEngine.PostEvent(djmpEvent, gameObject);
        }

        /// <summary>
        /// Call left dash event
        /// </summary>
        public void DashL()
        {
            AkSoundEngine.PostEvent("Play_Dash_L", gameObject);
        }
        
        /// <summary>
        /// Call right dash event
        /// </summary>
        public void DashR()
        {
            AkSoundEngine.PostEvent("Play_Dash_R", gameObject);
        }

        /// <summary>
        /// Call glide event
        /// </summary>
        public void PlayGlide()
        {
            AkSoundEngine.PostEvent("Play_Yellow_Quartz", gameObject);
        }

        /// <summary>
        /// Call stop glide event
        /// </summary>
        public void StopGlide()
        {
            AkSoundEngine.PostEvent("Stop_Yellow_Quartz", gameObject);
        }
        
        /// <summary>
        /// Call death event
        /// </summary>
        public void Death()
        {
            AkSoundEngine.PostEvent("Play_Death", gameObject);
        }

        /// <summary>
        /// Set the floor material sound
        /// </summary>
        private void CheckMaterial()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Default"));

            if (!hit.collider) return;
            
            GameObject other = hit.collider.gameObject;

            AkSoundEngine.PostEvent(
                other.TryGetComponent(out PlatformController plat) ? plat.matSoundEvent : floorSwitchEvent, gameObject);
        }
    }
}