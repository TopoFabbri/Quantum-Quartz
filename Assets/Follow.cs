using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float maxRotationSpeed = 200f;
    [SerializeField] private float stopDistance = 1f;

    private void Update()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) > stopDistance)
            transform.position = Vector2.MoveTowards(transform.position, target.position, maxSpeed * Time.deltaTime);
    }
}