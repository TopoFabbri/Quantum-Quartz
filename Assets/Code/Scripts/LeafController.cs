using UnityEngine;

public class LeafController : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            PlayPs();
    }

    private void PlayPs()
    {
        if (ps.isEmitting)
            return;
        
        ps.Play();
    }
}
