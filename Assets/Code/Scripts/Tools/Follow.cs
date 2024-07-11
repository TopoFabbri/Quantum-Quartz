using UnityEngine;

namespace Code.Scripts.Tools
{
    public class Follow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform rotator;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float maxRotationSpeed = 200f;
        [SerializeField] private float stopDistance = 1f;
        

        private void Update()
        {
            Vector2 direction = (target.position - transform.position).normalized;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            rotator.rotation =
                Quaternion.RotateTowards(rotator.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);

            Vector2 diff = (target.position - transform.position).normalized;

            if (Vector2.Distance(transform.position, target.position) > stopDistance)
                transform.position = Vector2.MoveTowards(transform.position, target.position, maxSpeed * Time.deltaTime);
            else if (Vector2.Distance(transform.position, target.position) < stopDistance)
                transform.position = Vector2.MoveTowards(transform.position, (Vector2)target.position - diff * stopDistance,
                    maxSpeed * Time.deltaTime);

            if (transform.position.y < target.position.y)
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, target.position.y), maxSpeed * Time.deltaTime);
        }
    }
}