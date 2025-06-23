using System.Collections;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class DroneController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 targetOffset;
        [SerializeField] private float maxDistance = 5f;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float yAdjustment = 1;
        [SerializeField] private AnimationCurve speedCurve;
        [SerializeField] private float maxRotationSpeed = 200f;
        [SerializeField] private float collisionTime = 3f;
        [SerializeField] private float minCollisionDist = 0.5f;
        [SerializeField] private Transform spotlight;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;

        private bool colliding;
        private int defLayer;

        private void Start()
        {
            defLayer = gameObject.layer;
        }

        private void FixedUpdate()
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
                Vector2 offset = target.position - transform.position;
                offset += targetOffset;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, maxRotationSpeed * Time.deltaTime);

                Vector2 direction = offset.normalized;
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                spotlight.rotation = Quaternion.RotateTowards(spotlight.rotation, Quaternion.Euler(0, 0, targetAngle), maxRotationSpeed * Time.deltaTime);


                float offsetDist = offset.magnitude / maxDistance;
                float speed = maxSpeed * (offsetDist <= 1 ? speedCurve.Evaluate(offsetDist) : offsetDist);
                rb.velocity = offset.normalized * speed + (offset.y < 0 ? 0 : yAdjustment) * Vector2.up * offset.normalized.y;
                rb.angularVelocity = 0;
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