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

        private void Start()
        {
            if (UnityEngine.Camera.main != null)
                camera = UnityEngine.Camera.main.GetComponent<CameraController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                camera.MoveTo(cameraPosition.position);
        }
    }
}