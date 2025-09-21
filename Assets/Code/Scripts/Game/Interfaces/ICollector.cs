using System;
using UnityEngine;

namespace Code.Scripts.Game.Interfaces
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

        protected Action _OnPausePickup { get; set; }
        public event Action OnPausePickup
        {
            add => _OnPausePickup += value;
            remove => _OnPausePickup -= value;
        }

        protected Action _OnCancelPickup { get; set; }
        public event Action OnCancelPickup
        {
            add => _OnCancelPickup += value;
            remove => _OnCancelPickup -= value;
        }

        public void AdvancePickup(float deltaTime)
        {
            _OnAdvancePickup?.Invoke(deltaTime);
        }

        public void PausePickup()
        {
            _OnPausePickup?.Invoke();
        }

        public void CancelPickup()
        {
            _OnCancelPickup?.Invoke();
        }

        public abstract Rigidbody2D GetFollowObject(Rigidbody2D rb);
    }
}