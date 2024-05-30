using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Platforms
{
    [Serializable]
    public struct Position
    {
        public Vector2 pos;
        public float rotation;
    }

    public class ObjMovement : MonoBehaviour
    {
        [SerializeField] private List<Position> points = new();
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool overTime;
        [SerializeField] private float time = 1f;

        private int curPoint;
        private Position initPos;
        private Transform player;
        private float timer;

        private void Start()
        {
            initPos.pos = transform.position;
            initPos.rotation = transform.rotation.z;
        }

        private void Update()
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
                transform.position = Vector2.Lerp(fromPos.pos, toPos.pos, timer / time);
                transform.rotation = Quaternion.Slerp(prevRot, nextRot, timer / time);
                
                timer += Time.deltaTime;

                while (timer >= time)
                {
                    curPoint = (curPoint + 1) % relativePoints.Count;
                    timer -= time;
                }
            }
            else
            {
                if ((Vector2)transform.position == relativePoints[curPoint].pos)
                    curPoint = (curPoint + 1) % relativePoints.Count;

                transform.position = Vector2.MoveTowards(transform.position, toPos.pos, speed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(prevRot, nextRot,
                    ((Vector2)transform.position - fromPos.pos).magnitude / (toPos.pos - fromPos.pos).magnitude);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (initPos.pos == Vector2.zero)
            {
                initPos.pos = transform.position;
                initPos.rotation = transform.rotation.z;
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

                Gizmos.DrawLine(relativePoints[i].pos,
                    (Vector3)relativePoints[i].pos +
                    Quaternion.Euler(0f, 0f, relativePoints[i].rotation) * Vector2.up * rotLength);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(relativePoints[i].pos,
                    i < relativePoints.Count - 1 ? relativePoints[i + 1].pos : relativePoints[0].pos);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        public void AddPlayer(Transform player)
        {
            this.player = player;
            this.player.parent = transform;
        }

        private void OnDisable()
        {
            if (!player)
                return;

            if (player.parent)
                player.parent = null;

            player = null;
        }
    }
}