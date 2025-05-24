using Code.Scripts.Animation;
using Code.Scripts.FSM;
using Code.Scripts.Player;
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
        protected readonly EdgeSettings edgeSettings;

        private readonly SharedContext sharedContext;
        private readonly FsmAnimationController animator;
        
        public EdgeState(T id, EdgeSettings stateSettings, SharedContext sharedContext, FsmAnimationController animator) : base(id)
        {
            this.edgeSettings = stateSettings;
            this.sharedContext = sharedContext;
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
            Vector2 startPosition = (Vector2)sharedContext.Transform.position + edgeSettings.edgeCheckOffset + Vector2.right * edgeSettings.edgeCheckDis;
            RaycastHit2D edge = Physics2D.Raycast(startPosition, Vector2.down, edgeSettings.edgeCheckLength, edgeSettings.edgeLayer);
            
            if (edgeSettings.shouldDraw)
                Debug.DrawLine(startPosition, startPosition + Vector2.down * edgeSettings.edgeCheckLength, edge.transform ? Color.green : Color.red);

            if (!edge)
                return false;
            
            if (!edge.transform.CompareTag("Floor") && !edge.transform.CompareTag("Platform"))
                return false;
            
            animator.SetEdgeSide(true);
            return true;
        }
        
        /// <summary>
        /// Show if player is on left edge
        /// </summary>
        /// <returns>True if player has an edge to the left</returns>
        private bool GetLeftEdge()
        {
            Vector2 startPosition = (Vector2)sharedContext.Transform.position + edgeSettings.edgeCheckOffset + Vector2.left * edgeSettings.edgeCheckDis;
            RaycastHit2D edge = Physics2D.Raycast(startPosition, Vector2.down, edgeSettings.edgeCheckLength, edgeSettings.edgeLayer);
            
            if (edgeSettings.shouldDraw)
                Debug.DrawLine(startPosition, startPosition + Vector2.down * edgeSettings.edgeCheckLength, edge.transform ? Color.green : Color.red);

            if (!edge)
                return false;
            
            if (!edge.transform.CompareTag("Floor") && !edge.transform.CompareTag("Platform"))
                return false;
            
            animator.SetEdgeSide(false);
            return true;
        }
    }
}