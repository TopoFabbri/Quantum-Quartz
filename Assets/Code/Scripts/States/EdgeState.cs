using Code.Scripts.Animation;
using Code.Scripts.FSM;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Edge state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EdgeState<T> : BaseState<T>
    {
        private readonly Transform transform;
        private readonly FsmAnimationController animator;
        
        private EdgeSettings EdgeSettings => settings as EdgeSettings;
        
        public EdgeState(T id, StateSettings.StateSettings settings, Transform transform, FsmAnimationController animator) : base(id, settings)
        {
            this.transform = transform;
            this.animator = animator;
        }

        public bool IsOnEdge()
        {
            return GetRightEdge() ^ GetLeftEdge();
        }
        
        /// <summary>
        /// Show if player is on right edge
        /// </summary>
        /// <returns>True if player has an edge to the right</returns>
        private bool GetRightEdge()
        {
            Vector2 startPosition = (Vector2)transform.position + EdgeSettings.edgeCheckOffset + Vector2.right * EdgeSettings.edgeCheckDis;
            RaycastHit2D edge = Physics2D.Raycast(startPosition, Vector2.down, EdgeSettings.edgeCheckLength, EdgeSettings.edgeLayer);
            
            if (EdgeSettings.shouldDraw)
                Debug.DrawLine(startPosition, startPosition + Vector2.down * EdgeSettings.edgeCheckLength, edge.transform ? Color.green : Color.red);

            if (!edge) return false;
            
            if (!edge.transform.CompareTag("Floor") && !edge.transform.CompareTag("Platform")) return false;
            
            animator.SetEdgeSide(true);
            return true;
        }
        
        /// <summary>
        /// Show if player is on left edge
        /// </summary>
        /// <returns>True if player has an edge to the left</returns>
        private bool GetLeftEdge()
        {
            Vector2 startPosition = (Vector2)transform.position + EdgeSettings.edgeCheckOffset + Vector2.left * EdgeSettings.edgeCheckDis;
            RaycastHit2D edge = Physics2D.Raycast(startPosition, Vector2.down, EdgeSettings.edgeCheckLength, EdgeSettings.edgeLayer);
            
            if (EdgeSettings.shouldDraw)
                Debug.DrawLine(startPosition, startPosition + Vector2.down * EdgeSettings.edgeCheckLength, edge.transform ? Color.green : Color.red);

            if (!edge) return false;
            
            if (!edge.transform.CompareTag("Floor") && !edge.transform.CompareTag("Platform")) return false;
            
            animator.SetEdgeSide(false);
            return true;
        }
    }
}