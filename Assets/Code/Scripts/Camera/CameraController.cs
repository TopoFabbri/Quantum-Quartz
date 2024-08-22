using System.Collections;
using Code.Scripts.Input;
using UnityEngine;

namespace Code.Scripts.Camera
{
    /// <summary>
    /// Camera controller
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private float moveDis = 10f;
        [SerializeField] private float inputSpeed = 2f;

        private Vector3 cameraPosition;
        private Vector2 offsetByInput;
        private float currentDis = 0f;

        private bool isMoving;

        private void Awake()
        {
            cameraPosition = transform.position;
        }

        private void OnEnable()
        {
            InputManager.MoveCam += OnMoveCam;
        }

        private void OnDisable()
        {
            InputManager.MoveCam -= OnMoveCam;
        }

        private void LateUpdate()
        {
            if (!isMoving)
            {
                if (offsetByInput.magnitude > 0)
                    currentDis = Mathf.Clamp01(currentDis + Time.deltaTime * inputSpeed);
                else
                    currentDis = 0;

                transform.position = Vector3.Lerp(cameraPosition, cameraPosition + (Vector3)offsetByInput, currentDis);
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, cameraPosition, Time.deltaTime * speed);

            if (transform.position == cameraPosition)
                isMoving = false;
        }

        /// <summary>
        /// Start camera movement towards position
        /// </summary>
        /// <param name="position"></param>
        public void MoveTo(Vector3 position)
        {
            cameraPosition = position;
            isMoving = true;
        }

        /// <summary>
        /// Called when camera movement is changed
        /// </summary>
        /// <param name="input">Input value</param>
        private void OnMoveCam(Vector2 input)
        {
            offsetByInput = input * moveDis;
        }

        /// <summary>
        /// Start camera shake
        /// </summary>
        /// <param name="duration">Shake duration</param>
        /// <param name="magnitude">Shake magnitude</param>
        public void Shake(float duration, float magnitude)
        {
            StartCoroutine(ShakeForDuration(duration, magnitude));
        }

        private IEnumerator ShakeForDuration(float duration, float magnitude)
        {
            Vector3 originalPos = transform.localPosition;
            Quaternion originalRotation = transform.localRotation;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = originalPos + new Vector3(x, y, 0f);

                float rotationX = Random.Range(-1f, 1f) * magnitude;
                float rotationY = Random.Range(-1f, 1f) * magnitude;

                transform.localRotation = originalRotation * Quaternion.Euler(rotationX, rotationY, 0f);

                yield return null;
            }

            transform.localPosition = originalPos;
            transform.localRotation = originalRotation;
        }
    }
}