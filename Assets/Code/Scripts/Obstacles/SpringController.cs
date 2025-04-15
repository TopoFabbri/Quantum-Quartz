using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class SpringController : MonoBehaviour
    {
        [SerializeField] private float force = 10f;
        [SerializeField] private Animator animator;
        [SerializeField] private PlatformEffector2D effector2D;
        
        [SerializeField] private string activateTrigger = "Activate";
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.enabled)
                return;
            
            if (!collision.transform.CompareTag("Player")) return;

            Activate(collision);
        }

        private void Activate(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(out Rigidbody2D rb)) return;

            rb.velocity = Vector2.Reflect(rb.velocity, transform.up) + (Vector2)transform.up * force;
            
            animator.SetTrigger(activateTrigger);
        }
    }
}