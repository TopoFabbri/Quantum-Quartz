using System.Collections.Generic;
using Code.Scripts.Interfaces;
using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class SpringController : MonoBehaviour
    {
        [Header("Spring:")] [SerializeField] private float force = 20f;
        [SerializeField] private Vector2 addedForce = new(0f, 10f);
        [SerializeField] private ForceMode2D forceMode;

        [Header("Animation:")] [SerializeField]
        private Animator animator;

        [SerializeField] private string activateTrigger = "Activate";

        [Header("Draw:")] [SerializeField] private Color color = Color.green;
        [SerializeField] private float scaleLength = 1f;
        [SerializeField] private float arrowSize = .2f;

        private readonly List<ISpringable> springables = new();

        private void OnDrawGizmosSelected()
        {
            Vector3 start = transform.position;
            Vector3 direction = transform.up * force + (Vector3)addedForce;

            direction *= scaleLength;

            Gizmos.color = color;
            Gizmos.DrawLine(start, start + direction);
            Gizmos.DrawLine(start + direction, start + direction - direction.normalized * arrowSize + Vector3.Cross(direction.normalized, Vector3.forward) * arrowSize);
            Gizmos.DrawLine(start + direction, start + direction - direction.normalized * arrowSize - Vector3.Cross(direction.normalized, Vector3.forward) * arrowSize);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            collision.TryGetComponent(out ISpringable springable);
            Activate(springable);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out ISpringable springable)) return;
            
            springables.Remove(springable);
        }

        private void Activate(ISpringable springable)
        {
            if (springables.Contains(springable)) return;

            springables.Add(springable);

            StartCoroutine(springable.Spring((Vector2)transform.up * force + addedForce, forceMode));

            animator.SetBool(activateTrigger, true);
        }
        
        private void EndAnimation()
        {
            animator.SetBool(activateTrigger, false);
        }

        public void PlaySound()
        {
            SfxController.PlaySpring(gameObject);
        }
    }
}