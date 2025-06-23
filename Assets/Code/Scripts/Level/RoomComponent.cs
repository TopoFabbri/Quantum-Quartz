using System;
using UnityEngine;

namespace Code.Scripts.Level
{
    public abstract class RoomComponent : MonoBehaviour
    {
        public event Action<RoomComponent> Destroyed;
        
        public abstract void OnActivate();
        public abstract void OnDeactivate();
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnStart() { }
        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}