using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Animation;
using Code.Scripts.Camera;
using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Input;
using Code.Scripts.Interfaces;
using Code.Scripts.Platforms;
using Code.Scripts.States;
using Code.Scripts.StateSettings;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage player actions
    /// </summary>
    [RequireComponent(typeof(PlayerState))]
    public class PlayerController : MonoBehaviour, IKillable, ISpringable
    {
        [SerializeField] private PlayerState playerState;

        // States (TRY TO REMOVE!)
        //private MoveState<string> moveState;     // Multiple -> Speed & ResetSpeed()
        //private DashState<string> dashState;     // Spring -> dashState.Interrupt();
        //private DjmpState<string> djmpState;     // Update & Spring -> djmpState.Reset();

        [SerializeField] private List<GameObject> flipObjects = new();
        
        [Header("Shake Settings")] [SerializeField]
        private float fallShakeMagnitudeMultiplier = 0.05f;

        [SerializeField] private float fallShakeDurationMultiplier = 0.05f;
        [SerializeField] private float minShakeValue = 0.5f;

        private void OnEnable()
        {
            PlayerState.OnFlip += OnFlipHandler;
        }

        private void OnDisable()
        {
            PlayerState.OnFlip -= OnFlipHandler;
        }

        private void FixedUpdate()
        {
            if (playerState.sharedContext.Rigidbody.velocity.y < 0f)
            {
                if (CamShakeCheck())
                {
                    float shakeValue = Mathf.Abs(playerState.sharedContext.Rigidbody.velocity.y) - minShakeValue;

                    playerState.sharedContext.CamController.Shake(shakeValue * fallShakeDurationMultiplier, shakeValue * fallShakeMagnitudeMultiplier);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.enabled && (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Platform")) && playerState.sharedContext.RecalculateIsGrounded())
            {
                if (collision.gameObject.TryGetComponent(out ObjectMovement obj))
                {
                    obj.AddPlayer(transform);
                }
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnCollisionEnter2D(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.enabled && (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Platform")))
            {
                if (collision.gameObject.TryGetComponent(out ObjectMovement obj) && transform.parent.Equals(obj.transform))
                {
                    transform.parent = null;
                }
            }
        }

        /// <summary>
        /// Flip character
        /// </summary>
        public void OnFlipHandler(bool facingRight)
        {
            foreach (GameObject flipObject in flipObjects)
            {
                flipObject.transform.Rotate(0f, 180f, 0f);
                flipObject.transform.localPosition = new Vector3(-flipObject.transform.localPosition.x, flipObject.transform.localPosition.y, flipObject.transform.localPosition.z);
            }
        }
        
        /// <summary>
        /// Check if player about to land
        /// </summary>
        /// <returns>True if ground is near</returns>
        private bool CamShakeCheck()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, LayerMask.GetMask("Default"));

            bool grounded = hit.collider && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform"));
            return grounded;
        }

        public Vector2 GetSpeed()
        {
            return playerState.sharedContext.speed;
        }
        
        public void Kill()
        {
            if (typeof(IDeathImmune).IsAssignableFrom(playerState.sharedContext.CurrentStateType))
                return;

            playerState.sharedContext.died = true;
        }

        public IEnumerator Spring(ISpringable.SpringDefinition springDefinition)
        {
            playerState.tempDashState.Interrupt();
            playerState.sharedContext.spring = springDefinition;
            
            yield return new WaitForFixedUpdate();

            playerState.tempDjmpState.Reset();
        }
    }
}