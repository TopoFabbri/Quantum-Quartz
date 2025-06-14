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
        public struct SpringDefinition
        {
            public Vector2 origin;
            public Vector2 force;

            public SpringDefinition(Vector3 origin, Vector2 force)
            {
                this.origin = origin;
                this.force = force;
            }
        }

        /// <summary>
        /// Applies a force to the player's rigidbody for spring-like movement
        /// </summary>
        /// <param name="springDefinition">Contains the information that defines the spring</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        public IEnumerator Spring(SpringDefinition springDefinition);
    }
}