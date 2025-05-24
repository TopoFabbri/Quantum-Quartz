using System.Collections;
using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Spawn state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpawnState<T> : BaseState<T>, IDeathImmune
    {
        protected readonly SpawnSettings spawnSettings;
        
        private readonly PlayerState.SharedContext sharedContext;
        
        public bool Ended { get; private set; }
        
        public SpawnState(T id, SpawnSettings stateSettings, PlayerState.SharedContext sharedContext) : base(id)
        {
            this.spawnSettings = stateSettings;
            this.sharedContext = sharedContext;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            sharedContext.Rigidbody.isKinematic = true;
            
            Reposition();
            
            Ended = false;
            sharedContext.MonoBehaviour.StartCoroutine(WaitAndEnd());
        }

        public override void OnExit()
        {
            base.OnExit();
            sharedContext.falling = false;

            sharedContext.Rigidbody.isKinematic = false;
        }

        /// <summary>
        /// Wait for duration and set to ended
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitAndEnd()
        {
            yield return new WaitForSeconds(spawnSettings.duration);
            Ended = true;
        }
        
        private void Reposition()
        {
            RaycastHit2D hit = Physics2D.Raycast(sharedContext.Transform.position, Vector2.down, 10f, LayerMask.GetMask("Default"));
            
            if (hit)
                sharedContext.Transform.position = hit.point + Vector2.up * spawnSettings.height;
        }
    }
}