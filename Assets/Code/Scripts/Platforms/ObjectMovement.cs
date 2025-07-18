using System;
using System.Collections.Generic;
using Code.Scripts.Level;
using UnityEngine;

namespace Code.Scripts.Platforms
{
    [Serializable]
    public struct Position
    {
        public Vector2 pos;
        public float rotation;
    }

    public class ObjectMovement : RoomComponent
    {
        [SerializeField] private List<Position> points = new();
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool overTime;
        [SerializeField] private float time = 1f;

        private int curPoint;
        private Position initPos;
        private Transform player;
        private float timer;
        private ColorObjectController colorObj;

        private void Start()
        {
            initPos.pos = transform.position;
            initPos.rotation = transform.rotation.eulerAngles.z;
            gameObject.TryGetComponent(out colorObj);
            if (colorObj)
            {
                colorObj.Toggled += ToggleColor;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                initPos = new Position { pos = transform.position, rotation = transform.rotation.eulerAngles.z };
            }

            List<Position> relativePoints = new() { initPos };

            foreach (Position point in points)
            {
                Position relativePoint;

                relativePoint.pos = initPos.pos + point.pos;
                relativePoint.rotation = point.rotation;

                relativePoints.Add(relativePoint);
            }

            for (int i = 0; i < relativePoints.Count; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(relativePoints[i].pos, 0.1f);

                float rotLength = .5f;

                Gizmos.DrawLine(relativePoints[i].pos, (Vector3)relativePoints[i].pos + Quaternion.Euler(0f, 0f, relativePoints[i].rotation) * Vector2.up * rotLength);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(relativePoints[i].pos, i < relativePoints.Count - 1 ? relativePoints[i + 1].pos : relativePoints[0].pos);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        public void AddPlayer(Transform player)
        {
            this.player = player;
            this.player.parent = transform;
        }

        public void RemovePlayer(Transform player)
        {
            if (player.parent == transform)
            {
                Debug.Log("Remove Player");
                player.parent = null;
                this.player = null;
            }
        }

        private void OnEnable()
        {
            if (colorObj)
            {
                colorObj.Toggled += ToggleColor;
            }
        }

        private void OnDisable()
        {
            if (colorObj)
            {
                colorObj.Toggled -= ToggleColor;
            }

            if (player)
            {
                RemovePlayer(player);
            }
        }

        private void ToggleColor(bool enabled)
        {
            if (!enabled && player)
            {
                RemovePlayer(player);
            }
        }

        public override void OnActivate() {}

        public override void OnDeactivate()
        {
            curPoint = 0;
            timer = 0f;
            transform.position = initPos.pos;
            transform.rotation = Quaternion.Euler(0f, 0f, initPos.rotation);
        }

        public override void OnFixedUpdate()
        {
            List<Position> relativePoints = new() { initPos };

            foreach (Position point in points)
            {
                Position relativePoint;

                relativePoint.pos = initPos.pos + point.pos;
                relativePoint.rotation = point.rotation;

                relativePoints.Add(relativePoint);
            }

            Position fromPos = relativePoints[curPoint == 0 ? relativePoints.Count - 1 : curPoint - 1];
            Position toPos = relativePoints[curPoint];

            Quaternion prevRot = Quaternion.Euler(0f, 0f, fromPos.rotation);
            Quaternion nextRot = Quaternion.Euler(0f, 0f, toPos.rotation);

            if (overTime)
            {
                if (time <= 0f)
                    time = 1f;

                transform.position = Vector2.Lerp(fromPos.pos, toPos.pos, timer / time);
                transform.rotation = Quaternion.Slerp(prevRot, nextRot, timer / time);

                timer += Time.fixedDeltaTime;

                while (timer >= time)
                {
                    curPoint = (curPoint + 1) % relativePoints.Count;
                    timer -= time;
                }
            }
            else
            {
                float moveAmount = speed * Time.fixedDeltaTime;

                Vector2 prevPos = transform.position;

                transform.position = Vector2.MoveTowards(transform.position, relativePoints[curPoint].pos, moveAmount);

                moveAmount -= Vector2.Distance(prevPos, relativePoints[curPoint].pos);

                if (moveAmount > 0f)
                {
                    curPoint = (curPoint + 1) % relativePoints.Count;

                    transform.position = Vector2.MoveTowards(transform.position, relativePoints[curPoint].pos, moveAmount);
                }

                float distance = (toPos.pos - fromPos.pos).magnitude;
                
                if (distance > 0f)
                {
                    transform.rotation = Quaternion.Slerp(prevRot, nextRot, ((Vector2)transform.position - fromPos.pos).magnitude / distance);
                }
            }
        }
    }
}