using Code.Scripts.Game.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Game.Obstacles
{
    /// <summary>
    /// Manage laser actions
    /// </summary>
    public class LaserController : RoomComponent
    {
        private enum LaserState
        {
            Offset,
            Off,
            Windup,
            On
        }

        [SerializeField] private float maxDis = 20f;
        [SerializeField] private float width = .8f;
        [SerializeField] private LayerMask mask;

        [SerializeField] private Transform origin;
        [SerializeField] private Transform end;
        [SerializeField] private LineRenderer line;
        [SerializeField] private Animator animator;

        [SerializeField] private bool useTime;
        [SerializeField] private float timeOn;
        [SerializeField] private float timeOff;
        [SerializeField] private float timeOffset;

        private LaserState state = LaserState.Offset;
        private readonly Dictionary<LaserState, Action> stateHandlers = new();

        private const float WindupTime = 0.6f;

        private float time;
        private static readonly int AnimatorOn = Animator.StringToHash("On");

        private void Awake()
        {
            stateHandlers.Add(LaserState.Offset, OffsetHandler);
            stateHandlers.Add(LaserState.Off, OffHandler);
            stateHandlers.Add(LaserState.Windup, WindupHandler);
            stateHandlers.Add(LaserState.On, OnHandler);

            if (useTime) return;
            
            timeOff = 0f;
            timeOffset = 0f;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            stateHandlers.Clear();
        }

        private void FindCollisionPoint()
        {
            IKillable killable;
            RaycastHit2D hit;
            float laserLength = 0;

            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            hits.AddRange(Physics2D.BoxCastAll(origin.position, new Vector2(width, 0.1f), 0, origin.right, maxDis, mask));

            if (hits.Count <= 0)
            {
                end.position = origin.position + origin.right * maxDis;
                end.rotation = origin.rotation;
                return;
            }

            for (int i = 0; i < hits.Count; i++)
            {
                hit = hits[i];
                Vector2 hitNormal = hit.normal;
                Collider2D hitCol = hit.collider;
                Debug.DrawLine(hit.point, hit.point + hitNormal, Color.magenta);

                if (hit.collider.TryGetComponent(out killable))
                {
                    killable.Kill();
                }

                laserLength = Vector2.Dot(hit.point - (Vector2)origin.position, hitNormal) / Vector2.Dot(hitNormal, origin.right);

                hit = Physics2D.Raycast(origin.position, origin.right, laserLength, mask);
                if (hit.collider && hit.distance < laserLength)
                {
                    laserLength = hit.distance;
                    hitNormal = hit.normal;
                    hitCol = hit.collider;
                    Debug.DrawLine(hit.point, hit.point + hitNormal, Color.magenta);
                }
                end.position = origin.position + origin.right * laserLength;
                end.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

                Vector2 closestPoint = hitCol.ClosestPoint(end.position);
                if ((closestPoint - (Vector2)end.position).magnitude <= width)
                {
                    break;
                }
                else if (i == hits.Count - 1)
                {
                    hits.AddRange(Physics2D.BoxCastAll(end.position + origin.right * 0.001f, new Vector2(width, 0.1f), 0, origin.right, maxDis - hit.distance, mask));
                }
            }

            Vector2 rayOrigin = origin.position + origin.up * width / 2f;

            hit = Physics2D.Raycast(rayOrigin, origin.right, laserLength, mask);
            Debug.DrawRay(rayOrigin, laserLength * origin.right, Color.green);

            if (hit.collider && hit.collider.TryGetComponent(out killable)) {
                killable.Kill();
            }

            rayOrigin -= (Vector2)origin.up * width;
            hit = Physics2D.Raycast(rayOrigin, origin.right, laserLength, mask);
            Debug.DrawRay(rayOrigin, laserLength * origin.right, Color.green);

            if (hit.collider && hit.collider.TryGetComponent(out killable))
            {
                killable.Kill();
            }
        }

        private void TurnOn()
        {
            animator.SetBool(AnimatorOn, true);
        }

        private void TurnOff()
        {
            animator.SetBool(AnimatorOn, false);
        }

        public override void OnActivate()
        {
            if (useTime) return;
            
            TurnOn();
        }

        public override void OnDeactivate()
        {
            TurnOff();
            state = LaserState.Offset;
            time = 0f;
            line.enabled = false;
            end.gameObject.SetActive(false);
        }

        public override void OnLateUpdate()
        {
            ManageLaserState();
            ManageLaserDisplay();
        }

        private void ManageLaserDisplay()
        {
            if (state != LaserState.On)
            {
                line.enabled = false;
                end.gameObject.SetActive(false);
                return;
            }

            line.enabled = true;
            end.gameObject.SetActive(true);

            FindCollisionPoint();

            line.SetPosition(0, origin.position);
            line.SetPosition(1, end.position);
        }

        private void ManageLaserState()
        {
            time += Time.deltaTime;

            stateHandlers[state]?.Invoke();
        }

        private void OffsetHandler()
        {
            if (time < timeOffset) return;

            state = LaserState.Windup;
            time -= timeOffset;
            TurnOn();
        }

        private void OffHandler()
        {
            if (time < timeOff) return;
            
            state = LaserState.Windup;
            time -= timeOff;
            TurnOn();
        }
        
        private void WindupHandler()
        {
            if (time < WindupTime) return;

            state = LaserState.On;
            time -= WindupTime;
        }

        private void OnHandler()
        {
            if (!useTime) return;
            if (time < timeOn) return;

            state = LaserState.Off;
            time -= timeOn;
            TurnOff();
        }
    }
}