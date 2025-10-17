using System;
using System.Collections.Generic;
using UnityEngine;
using Code.Scripts.Player;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Scripts.Game
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D roomTrigger;
        [SerializeField] private BoxCollider2D objectsBox;
        [SerializeField] private Vector2 moveRange;
        [SerializeField] private Color rectangleColor = Color.magenta;
        [SerializeField] private Vector2 followOffset;

        private readonly List<RoomComponent> roomComponents = new();

        private Vector2 camRange;

        private static Camera _cam;
        private static Room _activeRoom;

        public static event Action<Room, Room> Changed;

        private static Camera Cam
        {
            get
            {
                if (!_cam)
                    _cam = Camera.main;

                return _cam;
            }
        }

        public static Room Active
        {
            get => _activeRoom;
            private set
            {
                if (_activeRoom == value)
                    return;

                Changed?.Invoke(_activeRoom, value);
                
                _activeRoom = value;
            }
        }

        public Vector2 MoveRange => moveRange;
        public Vector2 FollowOffset => followOffset;

        private static Transform Player { get; set; }

        private void Start()
        {
            Bounds bounds = objectsBox.bounds;

            Collider2D[] hits = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);

            foreach (Collider2D hit in hits)
            {
                RoomComponent[] otherRoomComponents = hit.GetComponents<RoomComponent>();

                foreach (RoomComponent otherRoomComponent in otherRoomComponents)
                {
                    if (otherRoomComponent && !roomComponents.Contains(otherRoomComponent))
                    {
                        roomComponents.Add(otherRoomComponent);
                        otherRoomComponent.Destroyed += OnDestroyedRoomComponent;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (_activeRoom == this)
                _activeRoom = null;
            
            OnDeactivate();
        }

        private void OnDestroyedRoomComponent(RoomComponent obj)
        {
            if (!roomComponents.Contains(obj)) return;

            roomComponents.Remove(obj);
            obj.Destroyed -= OnDestroyedRoomComponent;
        }

        private void OnDrawGizmosSelected()
        {
            CalculateCameraRange();

            Gizmos.color = rectangleColor;
            Gizmos.DrawWireCube(transform.position, camRange * 2f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger || !other.CompareTag("Player"))
                return;

            if (!Player)
                Player = other.transform;

            Active = this;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.isTrigger || !other.CompareTag("Player") || _activeRoom != this)
                return;

            if (other.TryGetComponent(out PlayerController player) && !player.IsDead)
            {
                OnDeactivate();
                _activeRoom = null;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!Active)
            {
                OnTriggerEnter2D(other);
            }
        }

        private void Update()
        {
            if (Active != this)
                return;

            CalculateCameraRange();

            foreach (RoomComponent roomComponent in roomComponents)
                roomComponent.OnUpdate();
        }

        private void LateUpdate()
        {
            if (Active != this)
                return;

            foreach (RoomComponent roomComponent in roomComponents)
                roomComponent.OnLateUpdate();
        }

        private void FixedUpdate()
        {
            if (Active != this)
                return;

            foreach (RoomComponent roomComponent in roomComponents)
                roomComponent.OnFixedUpdate();
        }

        private void CalculateCameraRange()
        {
            if (Cam)
                camRange = (Cam.ViewportToWorldPoint(new Vector3(1, 1, Cam.nearClipPlane)) - Cam.ViewportToWorldPoint(new Vector3(0, 0, Cam.nearClipPlane))) / 2f;

            camRange += moveRange;

            if (roomTrigger.size == camRange * 2f - Vector2.one)
                return;

            roomTrigger.size = camRange * 2f - Vector2.one;
            objectsBox.transform.localScale = camRange * 2f;

#if UNITY_EDITOR
            EditorUtility.SetDirty(roomTrigger);
            EditorUtility.SetDirty(objectsBox);
            EditorUtility.SetDirty(this);
#endif
        }

        public void OnDeactivate()
        {
            foreach (RoomComponent roomComponent in roomComponents)
                roomComponent.OnDeactivate();
        }

        public void OnActivate()
        {
            foreach (RoomComponent roomComponent in roomComponents)
                roomComponent.OnActivate();
        }
    }
}