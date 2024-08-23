using System.Collections;
using UnityEngine;

namespace Code.Scripts.Tools
{
    public class Follow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform rotator;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float maxRotationSpeed = 200f;
        [SerializeField] private float stopDistance = 1f;
        [SerializeField] private float collisionTime = 3f;

        private static readonly int Broken = Animator.StringToHash("Broken");
        
        private bool colliding;

        private void Update()
        {
            animator.SetBool(Broken, colliding);
            if (colliding)
            {
                rb.gravityScale = 1f;
                return;
            }
            
            rb.gravityScale = 0f;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity,
                maxRotationSpeed * Time.deltaTime);

            Vector2 direction = (target.position - transform.position).normalized;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            rotator.rotation =
                Quaternion.RotateTowards(rotator.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);

            Vector2 diff = (target.position - transform.position).normalized;

            if (Vector2.Distance(transform.position, target.position) > stopDistance)
                transform.position =
                    Vector2.MoveTowards(transform.position, target.position, maxSpeed * Time.deltaTime);
            else if (Vector2.Distance(transform.position, target.position) < stopDistance)
                transform.position = Vector2.MoveTowards(transform.position,
                    (Vector2)target.position - diff * stopDistance,
                    maxSpeed * Time.deltaTime);

            if (transform.position.y < target.position.y)
                transform.position = Vector2.MoveTowards(transform.position,
                    new Vector2(transform.position.x, target.position.y), maxSpeed * Time.deltaTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            StartCoroutine(WaitAndResetRb(collisionTime));
            colliding = true;
        }

        /// <summary>
        /// Reset rigidbody after time
        /// </summary>
        /// <param name="time">Time until reset</param>
        /// <returns></returns>
        private IEnumerator WaitAndResetRb(float time)
        {
            yield return new WaitForSeconds(time);
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            colliding = false;
        }
    }
}