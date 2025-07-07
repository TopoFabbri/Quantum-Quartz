using Code.Scripts.Level;
using UnityEngine;

namespace Code.Scripts.Camera
{
    /// <summary>
    /// Switch screen trigger
    /// </summary>
    public class ScreenTrigger : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition;
        [SerializeField] private new CameraController camera;
        [SerializeField] private bool last;
        
        private void Start()
        {
            UnityEngine.Camera.main?.transform.parent?.TryGetComponent(out camera);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger || !other.CompareTag("Player"))
                return;

            if (last)
                TimeCounter.Stop();
            
            if (other.TryGetComponent(out DeathController deathController))
                deathController.CheckPoint(transform.position);
        }
    }
}