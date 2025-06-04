using UnityEngine;

namespace Code.Scripts.Level
{
    public abstract class RoomComponent : MonoBehaviour
    {
        public abstract void OnActivate();
        public abstract void OnDeactivate();
        public abstract void OnUpdate();
    }
}