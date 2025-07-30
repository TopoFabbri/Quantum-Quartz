using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Level;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Camera
{
    /// <summary>
    /// Camera controller
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private UnityEngine.Camera cam;

        [SerializeField] private Transform player;

        [Header("Settings")] [SerializeField] private float changeRoomSpeed = 20f;
        [SerializeField] private float followSpeed = 2f;
        
        private readonly Dictionary<int, bool> shakes = new();
        private readonly List<int> shakeIds = new();

        private bool inRoom = true;
        private bool positioned;
        private Vector3 originalPos;
        private Vector3 targetPos;

        public event Action MoveCam;
        public event Action StopCam;

        private Vector2 Center { get; set; }
        private Vector2 MoveRange { get; set; }
        private Vector2 FollowOffset { get; set; }

        private void Start()
        {
            originalPos = cam.transform.localPosition;
            positioned = false;
        }

        private void LateUpdate()
        {
            if (Room.Active)
            {
                Center = Room.Active.transform.position;
                MoveRange = Room.Active.MoveRange;
                FollowOffset = Room.Active.FollowOffset;
            }
            
            CalculateTargetPos();
            
            if (Room.Active && !positioned)
            {
                positioned = true;
                transform.position = targetPos;
            }
            
            if (IsCamInRoom())
                Follow();
            else
                SwitchRoom();
        }

        /// <summary>
        /// Move the camera to the room's center at a constant speed.
        /// </summary>
        private void SwitchRoom()
        {
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, changeRoomSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Move the camera to the player's position, but only inside the room's boundaries
        /// </summary>
        private void Follow()
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }

        private void CalculateTargetPos()
        {
            targetPos = player.position + (Vector3)FollowOffset;
            
            targetPos.x = Mathf.Clamp(targetPos.x, Center.x - MoveRange.x, Center.x + MoveRange.x);
            targetPos.y = Mathf.Clamp(targetPos.y, Center.y - MoveRange.y, Center.y + MoveRange.y);
        }
        
        private bool IsCamInRoom()
        {
            bool newInRoom = transform.position.x >= Center.x - MoveRange.x &&
                             transform.position.x <= Center.x + MoveRange.x &&
                             transform.position.y >= Center.y - MoveRange.y &&
                             transform.position.y <= Center.y + MoveRange.y;

            if (inRoom && !newInRoom)
                MoveCam?.Invoke();

            if (!inRoom && newInRoom)
                StopCam?.Invoke();

            inRoom = newInRoom;
            return inRoom;
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
            float elapsed = 0.0f;

            while (elapsed < duration && shakes[shakeId])
            {
                elapsed += Time.deltaTime;

                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                cam.transform.localPosition = originalPos + new Vector3(x, y, 0f);

                yield return null;
            }

            shakes.Remove(shakeId);
            shakeIds.Remove(shakeId);
            
            cam.transform.localPosition = originalPos;
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