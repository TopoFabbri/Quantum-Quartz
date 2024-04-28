using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stopDistance = 1f;

    private void Update()
    {
        
        Vector2 direction = target.position + Vector3.up * 2f - transform.position;
        
        transform.rotation = Quaternion.Euler(0f, 0f, direction.x > 0f ? -90f : 90f);
        
        direction = direction.normalized * (direction.magnitude - stopDistance);
        transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + direction,
            speed * Time.deltaTime);
    }
}