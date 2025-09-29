using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.FSM
{
    /// <summary>
    /// Automatic controller for animator by states
    /// </summary>
    public class FsmAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string intParamName = "State";
        [SerializeField] private string boolParamName = "FacingRight";
        [SerializeField] private string edgeParamName = "RightEdge";
        [SerializeField] private string hangingParamName = "Hanging";
        
        private readonly Dictionary<string, int> animIdsByName = new();

        private void Start()
        {
            animator.keepAnimatorStateOnDisable = true;
        }

        /// <summary>
        /// Add animation id by name
        /// </summary>
        /// <param name="name">State name</param>
        /// <param name="id">Animation number</param>
        public void AddState(string name, int id)
        {
            animIdsByName.Add(name, id);
        }

        /// <summary>
        /// Handle state change
        /// </summary>
        /// <param name="stateName">New state name</param>
        public void OnStateChangedHandler(string stateName)
        {
            animator.SetInteger(intParamName, animIdsByName[stateName]);
        }

        /// <summary>
        /// Handle player flip direction
        /// </summary>
        /// <param name="facingRight">New direction</param>
        public void OnFlipHandler(bool facingRight)
        {
            animator.SetBool(boolParamName, facingRight);
        }

        /// <summary>
        /// Toggle pause animation
        /// </summary>
        /// <param name="pause"> True to pause</param>
        public void TogglePauseAnim(bool pause)
        {
            animator.speed = pause ? 0 : 1;
        }

        /// <summary>
        /// Set edge right or left
        /// </summary>
        /// <param name="right">True for right</param>
        public void SetEdgeSide(bool right)
        {
            animator.SetBool(edgeParamName, right);
        }

        public void SetHanging(bool hanging)
        {
            animator.SetBool(hangingParamName, hanging);
        }
    }
}
