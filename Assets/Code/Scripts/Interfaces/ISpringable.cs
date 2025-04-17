using System.Collections;
using UnityEngine;

namespace Code.Scripts.Interfaces
{
    /// <summary>
    /// Interface for objects that can exhibit spring-like behaviour
    /// Provides functionality to apply spring forces to objects with physics components
    /// </summary>
    public interface ISpringable
    {
        /// <summary>
        /// Applies a force to the player's rigidbody for spring-like movement
        /// </summary>
        /// <param name="force">The direction and magnitude of the force to apply</param>
        /// <param name="mode">The type of force to apply (Force, Impulse, etc.)</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        public IEnumerator Spring(Vector2 force, ForceMode2D mode);
    }
}