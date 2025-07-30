using System.Collections;
using Code.Scripts.Level;
using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class MissileLauncherController : RoomComponent
    {
        [Header("References")]
        [SerializeField] private MissileController missileController;
        [SerializeField] private Transform cannon;
        [SerializeField] private Transform gunpoint;
        [SerializeField] private Animator animator;
        
        private Transform target;
        private bool fired;
        private bool missileActive;
        private bool lockMovement;
        
        private static readonly int InterruptTrigger = Animator.StringToHash("Interrupt");
        private static readonly int FireTrigger = Animator.StringToHash("Fire");

        public void Fire()
        {
            missileController.Shoot(target, gunpoint);
            missileActive = true;
        }

        public void LockMovement()
        {
            lockMovement = true;
        }
        
        public void UnlockMovement()
        {
            lockMovement = false;
        }
        
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (target && !lockMovement)
                cannon.rotation = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            target = other.transform;
            
            if (fired) return;
            StartShoot();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            target = null;
            animator.SetTrigger(InterruptTrigger);
            
            if (!missileActive)
                fired = false;
        }

        private void OnMissileDestroyed()
        {
            fired = false;
            missileActive = false;
            
            if (target)
                StartShoot();
        }

        private void StartShoot()
        {
            fired = true;
            animator.SetTrigger(FireTrigger);
        }
        
        public override void OnActivate()
        {
            missileController.OnDestroyed += OnMissileDestroyed;
        }

        public override void OnDeactivate()
        {
            missileController.OnDestroyed -= OnMissileDestroyed;
        }
    }
}