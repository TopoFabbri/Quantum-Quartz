using UnityEngine;

namespace Code.Scripts.Tools
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        private static MonoBehaviourSingleton<T> _instance;

        public static T Instance
        {
            get 
            {
                if (!_instance)
                    _instance = FindObjectOfType<MonoBehaviourSingleton<T>>();

                if (_instance) return (T)_instance;

                GameObject obj = new() { name = typeof(T).Name };
                _instance = obj.AddComponent<T>();

                return (T)_instance;
            }
        }
        
        protected virtual void Initialize()
        {

        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            Initialize();
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}
