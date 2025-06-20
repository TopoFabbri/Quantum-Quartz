using System.Collections;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class DroneController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform spotlight;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float maxRotationSpeed = 200f;
        [SerializeField] private float stopDistance = 1f;
        [SerializeField] private float collisionTime = 3f;
        [SerializeField] private float minCollisionDist = 0.5f;

        private bool colliding;
        private int defLayer;

        private void Start()
        {
            defLayer = gameObject.layer;
        }

        private void Update()
        {
            if (colliding)
            {
                if (Vector2.Distance(transform.position, target.position) < minCollisionDist)
                {
                    gameObject.layer = defLayer;
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, maxRotationSpeed * Time.deltaTime);

                Vector2 direction = (target.position - transform.position).normalized;
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                spotlight.rotation = Quaternion.RotateTowards(spotlight.rotation, Quaternion.Euler(0, 0, targetAngle), maxRotationSpeed * Time.deltaTime);

                Vector2 destination = transform.position;
                if (Vector2.Distance(transform.position, target.position) > stopDistance)
                {
                    destination = target.position;
                }
                else if (Vector2.Distance(transform.position, target.position) < stopDistance)
                {
                    destination = (Vector2)target.position - direction * stopDistance;
                }
                transform.position = Vector2.MoveTowards(transform.position, destination, maxSpeed * Time.deltaTime);

                if (transform.position.y < target.position.y)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, target.position.y), maxSpeed * Time.deltaTime);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            StartCoroutine(WaitAndResetRb(collisionTime));
            colliding = true;
            rb.gravityScale = 1f;
            animator.SetBool("Broken", colliding);
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
            rb.gravityScale = 0f;
            animator.SetBool("Broken", colliding);
            gameObject.layer = defLayer;
        }
    }
}