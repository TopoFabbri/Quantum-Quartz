using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Obstacles
{
    public class SpringController : MonoBehaviour
    {
        [Header("Spring:")] [SerializeField] private float force = 20f;
        [SerializeField] private Vector2 addedForce = new(0f, 10f);

        [Header("Animation:")] [SerializeField]
        private Animator animator;

        [SerializeField] private string activateTrigger = "Activate";

        [Header("Draw:")] [SerializeField] private Color color = Color.green;
        [SerializeField] private float scaleLength = 1f;
        [SerializeField] private float arrowSize = .2f;

        private readonly List<Rigidbody2D> rbs = new();

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
            if (!collision.transform.CompareTag("Player")) return;

            Activate(collision);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out Rigidbody2D rb)) return;
            
            rbs.Remove(rb);
        }

        private void Activate(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out Rigidbody2D rb)) return;

            if (rbs.Contains(rb)) return;

            rbs.Add(rb);

            rb.velocity = Vector2.zero;
            
            StartCoroutine(AddForceOnFixedUpdate(rb, transform.up * force + (Vector3)addedForce, ForceMode2D.Impulse));

            animator.SetBool(activateTrigger, true);
        }
        
        private void EndAnimation()
        {
            animator.SetBool(activateTrigger, false);
        }

        private static IEnumerator AddForceOnFixedUpdate(Rigidbody2D rb, Vector2 forceToAdd, ForceMode2D forceMode)
        {
            yield return new WaitForFixedUpdate();
            
            rb.AddForce(forceToAdd, forceMode);
        }
    }
}