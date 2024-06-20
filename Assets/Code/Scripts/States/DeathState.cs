using System.Collections;
using Code.Scripts.FSM;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Die state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeathState<T> : BaseState<T>
    {
        protected DeathSettings DeathSettings => (DeathSettings)settings;
        
        private readonly Transform transform;
        private readonly Rigidbody2D rb;
        private readonly MonoBehaviour mb;
        
        public Vector2 Direction { get; set; }
        public bool Ended { get; private set; }
        
        public DeathState(T id, StateSettings.StateSettings settings, Transform transform, Rigidbody2D rb, MonoBehaviour mb) : base(id, settings)
        {
            this.transform = transform;
            this.rb = rb;
            this.mb = mb;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Ended = false;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            mb.StartCoroutine(WaitAndEnd());
        }

        public override void OnExit()
        {
            base.OnExit();
            
            rb.isKinematic = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            Vector2 target = (Vector2)transform.position + Direction;
            transform.position = Vector2.MoveTowards(transform.position, target, DeathSettings.maxSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// Wait for duration and set to ended
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitAndEnd()
        {
            yield return new WaitForSeconds(DeathSettings.duration);
            Ended = true;
        }
    }
}