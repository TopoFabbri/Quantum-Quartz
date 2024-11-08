using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [SerializeField] private float moveTimeOffset = 0.5f;

        private readonly Dictionary<int, bool> shakes = new();
        private List<int> shakeIds = new();

        private Vector3 cameraPosition;
        private float currentDis;

        private bool isMoving;

        public event Action MoveCam;
        public event Action StopCam;

        private void Awake()
        {
            cameraPosition = transform.position;
        }

        private void LateUpdate()
        {
            if (!isMoving)
                return;

            transform.position = Vector3.MoveTowards(transform.position, cameraPosition, Time.deltaTime * speed);

            if (transform.position == cameraPosition)
            {
                StartCoroutine(WaitAndToggleMoving(moveTimeOffset));
            }
        }

        /// <summary>
        /// Start camera movement towards position
        /// </summary>
        /// <param name="position"></param>
        public void MoveTo(Vector3 position)
        {
            if (cameraPosition == position)
                return;
            
            cameraPosition = position;
            StartCoroutine(WaitAndToggleMoving(moveTimeOffset));
        }

        /// <summary>
        /// Start camera shake
        /// </summary>
        /// <param name="duration">Shake duration</param>
        /// <param name="magnitude">Shake magnitude</param>
        public void Shake(float duration, float magnitude)
        {
            StopAllShakes();

            int i = 0;

            while (shakes.ContainsKey(i))
                i++;

            shakes.Add(i, true);
            shakeIds.Add(i);

            StartCoroutine(ShakeForDuration(duration, magnitude, i));
        }

        /// <summary>
        /// Shake camera for given duration and magnitude
        /// </summary>
        /// <param name="duration">Shake duration</param>
        /// <param name="magnitude">Shake magnitude</param>
        /// <returns></returns>
        private IEnumerator ShakeForDuration(float duration, float magnitude, int shakeId)
        {
            Vector3 originalPos = transform.localPosition;
            float elapsed = 0.0f;

            while (elapsed < duration && shakes[shakeId])
            {
                elapsed += Time.deltaTime;

                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = originalPos + new Vector3(x, y, 0f);

                yield return null;
            }

            shakes.Remove(shakeId);
            shakeIds.Remove(shakeId);
            transform.localPosition = originalPos;
        }

        /// <summary>
        /// Wait time and start move camera
        /// </summary>
        /// <param name="time"> Time to wait</param>
        /// <returns></returns>
        private IEnumerator WaitAndToggleMoving(float time)
        {
            if (isMoving)
            {
                isMoving = false;
                yield return new WaitForSeconds(time);
                StopCam?.Invoke();
            }
            else
            {
                MoveCam?.Invoke();
                yield return new WaitForSeconds(time);
                isMoving = true;
            }
        }

        /// <summary>
        /// Stop all camera shakes
        /// </summary>
        private void StopAllShakes()
        {
            foreach (int shakeId in shakeIds)
                shakes[shakeId] = false;
        }
    }
}