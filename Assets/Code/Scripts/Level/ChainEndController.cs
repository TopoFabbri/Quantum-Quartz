using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level
{
    public class ChainEndController : MonoBehaviour
    {
        [SerializeField] private float minForce = 2f;
        [SerializeField] private float maxForce = 3f;
        
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (!rb)
                rb = gameObject.AddComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
                rb.AddForce(Vector2.right * player.Speed * Random.Range(minForce, maxForce), ForceMode2D.Impulse);
        }
    }
}