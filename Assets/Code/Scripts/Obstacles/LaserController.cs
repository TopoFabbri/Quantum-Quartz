using System;
using System.Collections.Generic;
using Code.Scripts.Interfaces;
using Code.Scripts.Level;
using UnityEngine;

namespace Code.Scripts.Obstacles
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

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            stateHandlers.Clear();
        }

        private void FindCollisionPoint()
        {
            RaycastHit2D hit = Physics2D.BoxCast(origin.position, new Vector2(width, 0.1f), 0, origin.right, maxDis, mask);
            //RaycastHit2D hit = Physics2D.Raycast(origin.position, origin.right, maxDis, mask);

            float dis = hit ? hit.distance : maxDis;

            if (!hit.collider)
            {
                end.position = origin.position + origin.right * dis;
                end.rotation = origin.rotation;
                return;
            }

            end.position = origin.position + origin.right * Vector2.Dot(hit.point - new Vector2(origin.position.x, origin.position.y), origin.right);
            end.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (hit.collider.TryGetComponent(out IKillable killable))
                killable.Kill();

            hit = Physics2D.Raycast(origin.position + origin.up * width / 2f, origin.right, dis, mask);

            if (hit)
            {
                if (hit.collider.TryGetComponent(out killable))
                    killable.Kill();
            }

            hit = Physics2D.Raycast(origin.position - origin.up * width / 2f, origin.right, dis, mask);

            if (!hit)
                return;

            if (hit.collider.TryGetComponent(out killable))
                killable.Kill();
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

            line.SetPosition(0, origin.position + origin.right * 0.5f);
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