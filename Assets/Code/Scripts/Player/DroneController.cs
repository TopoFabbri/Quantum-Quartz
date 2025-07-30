using System;
using System.Collections;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class DroneController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 targetOffset;
        [SerializeField] private float maxDistance = 30;
        [SerializeField] private float maxSpeed = 4;
        [SerializeField] private float yAdjustment = 3;
        [SerializeField] private AnimationCurve speedCurve;
        [SerializeField] private float avoidanceDist = 6;
        [SerializeField] private AnimationCurve avoidanceCurve;
        [SerializeField] private float minImpulse = 10;
        [SerializeField] private float maxRotationSpeed = 150f;
        [SerializeField] private float collisionTime = 1.5f;
        [SerializeField] private float minCollisionDist = 2;
        [SerializeField] private Transform spotlight;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;

        private bool colliding;
        private int defLayer;
        private int collisionLayer;
        private Coroutine coroutine;
        private Transform overridePosition;
        private Action onOverridePositionReached;
        private float speedMultiplier = 1;

        private void Start()
        {
            defLayer = gameObject.layer;
            collisionLayer = LayerMask.NameToLayer("Default");
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
                    gameObject.layer = collisionLayer;
                }
            }
            else
            {
                Vector2 offset = (overridePosition != null ? overridePosition.position : target.position) - transform.position;
                offset += targetOffset;

                if (overridePosition != null && onOverridePositionReached != null && offset.magnitude < 0.001f)
                {
                    onOverridePositionReached.Invoke();
                    onOverridePositionReached = null;
                }

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, maxRotationSpeed * Time.deltaTime);

                float offsetDist;
                float speed;
                if (offset.magnitude > avoidanceDist || overridePosition != null)
                {
                    offsetDist = offset.magnitude / maxDistance;
                    speed = maxSpeed * (offsetDist <= 1 ? speedCurve.Evaluate(offsetDist) : offsetDist);
                }
                else
                {
                    offsetDist = offset.magnitude / avoidanceDist;
                    speed = maxSpeed * avoidanceCurve.Evaluate(offsetDist);
                }
                rb.velocity = offset.normalized * speed * speedMultiplier + (offset.y < 0 ? 0 : yAdjustment) * Vector2.up * offset.normalized.y;
                rb.angularVelocity = 0;

                Vector2 direction = (target.position - transform.position).normalized;
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                spotlight.rotation = Quaternion.RotateTowards(spotlight.rotation, Quaternion.Euler(0, 0, targetAngle), maxRotationSpeed * Time.deltaTime);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            if (!colliding && rb.velocity.magnitude < minImpulse)
            {
                rb.velocity = rb.velocity.normalized * minImpulse;
            }

            coroutine = StartCoroutine(WaitAndResetRb(collisionTime));
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
            coroutine = null;
        }

        public void GoToPosition(Transform position, float speedMult, Action onPositionReached = null)
        {
            overridePosition = position;
            speedMultiplier = position ? speedMult : 1;
            onOverridePositionReached = onPositionReached;
        }
    }
}