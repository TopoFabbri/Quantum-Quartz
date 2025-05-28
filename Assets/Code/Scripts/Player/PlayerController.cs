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

        private float MoveInput => playerState.sharedContext.Input;
        
        public float Speed => playerState.tempMoveState.Speed;

        private void OnEnable()
        {
            PlayerState.OnFlip += OnFlipHandler;
        }

        private void OnDisable()
        {
            PlayerState.OnFlip -= OnFlipHandler;
        }

        private void Update()
        {
            Type curStateType = playerState.sharedContext.CurrentStateType;
            // What do all these states have in common that require the Flip() to be called?
            if (curStateType != typeof(WallState<string>) && curStateType != typeof(WallJumpState<string>) && curStateType != typeof(GrabState<string>))
            {
                bool leftMove = Speed < 0f || (Speed == 0f && MoveInput < 0f);
                bool rightMove = Speed > 0f || (Speed == 0f && MoveInput > 0f);
                if (playerState.sharedContext.facingRight && leftMove || !playerState.sharedContext.facingRight && rightMove)
                {
                    playerState.Flip();
                }
            }
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
                if (collision.gameObject.TryGetComponent(out ObjMovement obj))
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
                if (collision.gameObject.TryGetComponent(out ObjMovement obj) && transform.parent.Equals(obj.transform))
                {
                    Debug.Log("Exit " + collision.gameObject);
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
        
        public void Kill()
        {
            if (typeof(IDeathImmune).IsAssignableFrom(playerState.sharedContext.CurrentStateType))
                return;

            playerState.sharedContext.died = true;
        }

        public IEnumerator Spring(Vector2 force, ForceMode2D mode)
        {
            //springState.Activate(); //Deactivated for now
            playerState.tempDashState.Interrupt();
            
            yield return new WaitForFixedUpdate();

            playerState.tempDjmpState.Reset();

            playerState.sharedContext.Rigidbody.velocity = Vector2.zero;
            playerState.sharedContext.Rigidbody.AddForce(force, mode);
        }
    }
}