using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Animation
{
    /// <summary>
    /// Automatic controller for animator by states
    /// </summary>
    public class FsmAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string intParamName = "State";
        [SerializeField] private string boolParamName = "FacingRight";
                
        private readonly Dictionary<string, int> animIdsByName = new();

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
    }
}