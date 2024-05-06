using System;
using Code.Scripts.Input;
using UnityEngine;
using UnityEngine.Serialization;

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
            if (offsetByInput.magnitude > 0)
                currentDis = Mathf.Clamp01(currentDis + Time.deltaTime * inputSpeed);
            else
                currentDis = 0;
            
            transform.position = Vector3.Lerp(cameraPosition, cameraPosition + (Vector3)offsetByInput, currentDis);

            Debug.Log("currentDis: " + currentDis);
            
            if (!isMoving) return;
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
    }
}