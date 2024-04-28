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
                if (_instance == null)
                    _instance = FindObjectOfType<MonoBehaviourSingleton<T>>();

                return (T)_instance;
            }
        }

        protected virtual void Initialize()
        {

        }

        private void Awake()
        {
            if (_instance != null)
                Destroy(this.gameObject);

            _instance = this;

            Initialize();
        }
    }
}
