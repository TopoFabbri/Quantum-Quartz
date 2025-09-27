using Code.Scripts.Game.Interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Code.Scripts.Game.Obstacles
{
    public class MissileController : RoomComponent
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float rotSpeed = 10f;
        [SerializeField] private float explosionRadius = 1f;
        
        [SerializeField] private Animator animator;
        
        private Transform target;

        public event Action OnDestroyed;
        
        private static readonly int Fire = Animator.StringToHash("Fire");
        private static readonly int Explode = Animator.StringToHash("Explode");

        public void Shoot(Transform target, Transform spawnTransform)
        {
            transform.position = spawnTransform.position;
            transform.rotation = spawnTransform.rotation;
            this.target = target;
            
            gameObject.SetActive(true);
            animator.SetTrigger(Fire);
            
            StartCoroutine(WaitAndDestroy());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            
            OnDestroyed?.Invoke();
        }
        
        public override void OnUpdate()
        {
            if (!target) return;
            
            Rotate();
            Move();
        }

        private void Rotate()
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }

        private void Move()
        {
            transform.Translate(Vector3.up * (speed * Time.deltaTime));
        }

        private void OnCollisionEnter2D()
        {
            Destroy();
        }

        private void Destroy()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            
            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out IKillable killable))
                    killable.Kill();
            }
            animator.SetTrigger(Explode);
            
            target = null;
        }
        
        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(lifetime);

            if (target)
            {
                Destroy();
            }
            else
            {
                Hide();
            }
        }

        public override void OnActivate() { }

        public override void OnDeactivate()
        {
            Destroy();
        }
    }
}