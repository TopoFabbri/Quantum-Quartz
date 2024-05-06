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
            if (UnityEngine.Camera.main != null)
                camera = UnityEngine.Camera.main.GetComponent<CameraController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (last)
                TimeCounter.Stop();
            
            camera.MoveTo(cameraPosition.position);
            
            if (other.TryGetComponent(out DeathController deathController))
                deathController.CheckPoint(transform.position);
        }
    }
}