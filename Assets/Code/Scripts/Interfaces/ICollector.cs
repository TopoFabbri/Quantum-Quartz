using System;
using UnityEngine;

namespace Code.Scripts.Interfaces
{
    /// <summary>
    /// Interface for an object that can pick up collectibles
    /// </summary>
    public interface ICollector
    {
        protected Action<float> _OnAdvancePickup { get; set; }
        public event Action<float> OnAdvancePickup
        {
            add => _OnAdvancePickup += value;
            remove => _OnAdvancePickup -= value;
        }

        protected Action _OnStopPickup { get; set; }
        public event Action OnStopPickup
        {
            add => _OnStopPickup += value;
            remove => _OnStopPickup -= value;
        }

        public void AdvancePickup(float deltaTime)
        {
            _OnAdvancePickup?.Invoke(deltaTime);
        }

        public void StopPickup()
        {
            _OnStopPickup?.Invoke();
        }

        public abstract Rigidbody2D GetFollowObject(Rigidbody2D rb);
    }
}