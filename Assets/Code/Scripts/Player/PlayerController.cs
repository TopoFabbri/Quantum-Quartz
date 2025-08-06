using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Input;
using Code.Scripts.Interfaces;
using Code.Scripts.Level;
using Code.Scripts.Platforms;
using Code.Scripts.States;
using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage player actions
    /// </summary>
    [RequireComponent(typeof(PlayerState))]
    public class PlayerController : MonoBehaviour, IKillable, ISpringable, ICollector
    {
        [SerializeField] private PlayerState playerState;
        [SerializeField] private List<GameObject> flipObjects = new();

        [HeaderPlus("Shake Settings")]
        [SerializeField] private float fallShakeMagnitudeMultiplier = 0.05f;
        [SerializeField] private float fallShakeDurationMultiplier = 0.05f;
        [SerializeField] private float minShakeValue = 0.5f;

        Action<float> ICollector._OnAdvancePickup { get; set; }
        Action ICollector._OnPausePickup { get; set; }
        Action ICollector._OnCancelPickup { get; set; }
        Rigidbody2D lastCollectible = null;
        bool inUnsafeState = false;
        private List<InteractableComponent> interactables = new List<InteractableComponent>();
        private SubstanceTrigger curSubstance = null;

        private void OnEnable()
        {
            PlayerState.OnFlip += OnFlipHandler;
            InputManager.Jump += OnJumpPressedHandler;
            InputManager.Interact += OnInteractHandler;
        }

        private void OnDisable()
        {
            PlayerState.OnFlip -= OnFlipHandler;
            InputManager.Jump -= OnJumpPressedHandler;
            InputManager.Interact -= OnInteractHandler;
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

            if (!typeof(IUnsafe).IsAssignableFrom(playerState.sharedContext.CurrentStateType))
            {
                inUnsafeState = false;
                (this as ICollector).AdvancePickup(Time.fixedDeltaTime);
            }
            else if (!inUnsafeState)
            {
                inUnsafeState = true;
                (this as ICollector).PausePickup();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.enabled && (col.gameObject.CompareTag("Floor") || col.gameObject.CompareTag("Platform")) && playerState.sharedContext.Collider.enabled && playerState.sharedContext.RecalculateIsGrounded())
            {
                if (col.gameObject.TryGetComponent(out ObjectMovement obj))
                {
                    obj.AddPlayer(transform);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            OnTriggerEnter2D(col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (transform.parent && col.enabled && (col.gameObject.CompareTag("Floor") || col.gameObject.CompareTag("Platform")))
            {
                if (transform.parent.Equals(col.transform))
                {
                    transform.parent = null;
                    if (col.gameObject.TryGetComponent(out ObjectMovement obj))
                    {
                        obj.RemovePlayer(transform);
                    }
                }
            }
        }

        private void OnInteractHandler(float value) => Interact(value);
        private bool Interact(float value)
        {
            if (value != 0 && interactables.Count > 0)
            {
                if (interactables[interactables.Count - 1].Interact())
                {
                    return true;
                }
                else
                {
                    interactables.RemoveAt(interactables.Count - 1);
                }
            }
            return false;
        }

        /// <summary>
        /// Handle player jump action
        /// </summary>
        private void OnJumpPressedHandler(float value)
        {
            if (Interact(value)) return;

            playerState.OnJump(value != 0);
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, playerState.sharedContext.SolidFilter.layerMask);

            bool grounded = hit.collider && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform"));
            return grounded;
        }

        public Vector2 GetSpeed()
        {
            return playerState.sharedContext.Speed;
        }

        public void SpawnAt(Vector2 pos)
        {
            SaveCheckpoint(pos);
            transform.position = pos;
        }

        public void SaveCheckpoint(Vector2 pos)
        {
            playerState.sharedContext.CheckpointPos = pos;
        }
        
        public void Kill()
        {
            if (typeof(IDeathImmune).IsAssignableFrom(playerState.sharedContext.CurrentStateType))
                return;

            (this as ICollector).CancelPickup();
            lastCollectible = null;
            playerState.sharedContext.died = true;
        }

        public IEnumerator Spring(ISpringable.SpringDefinition springDefinition)
        {
            playerState.sharedContext.spring = springDefinition;
            
            yield return new WaitForFixedUpdate();

            playerState.sharedContext.djmpAvailable = true;
        }

        public Rigidbody2D GetFollowObject(Rigidbody2D rb)
        {
            if (playerState.sharedContext.died)
            {
                return null;
            }

            Rigidbody2D output = lastCollectible == null ? playerState.sharedContext.Rigidbody : lastCollectible;
            lastCollectible = rb;
            return output;
        }

        public void EnterInteractable(InteractableComponent component)
        {
            interactables.Add(component);
        }

        public void ExitInteractable(InteractableComponent component)
        {
            interactables.Remove(component);
        }

        public void EnterSubstance(SubstanceTrigger substance)
        {
            playerState.sharedContext.CurMovementModifier = substance.Modifier;
            curSubstance = substance;
        }

        public void LeaveSubstance(SubstanceTrigger substance)
        {
            if (curSubstance && curSubstance.Equals(substance))
            {
                playerState.sharedContext.CurMovementModifier = null;
            }
        }
    }
}