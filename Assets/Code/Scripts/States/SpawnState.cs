using System.Collections;
using Code.Scripts.FSM;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Spawn state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpawnState<T> : BaseState<T>
    {
        protected readonly SpawnSettings spawnSettings;
        
        private readonly Transform transform;
        private readonly Rigidbody2D rb;
        private readonly MonoBehaviour mb;
        
        public bool Ended { get; private set; }
        
        public SpawnState(T id, SpawnSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb) : base(id)
        {
            this.spawnSettings = stateSettings;
            this.transform = transform;
            this.rb = rb;
            this.mb = mb;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            rb.isKinematic = true;
            
            Reposition();
            
            Ended = false;
            mb.StartCoroutine(WaitAndEnd());
        }

        public override void OnExit()
        {
            base.OnExit();
            
            rb.isKinematic = false;
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, LayerMask.GetMask("Default"));
            
            if(hit)
                transform.position = hit.point + Vector2.up * spawnSettings.height;
        }
    }
}