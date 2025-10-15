using Code.Scripts.Tools.EventSystem;
using UnityEngine;

namespace Code.Scripts.Game
{
    public class DjmpMarker : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string fillTrigger;
        [SerializeField] private string depleteTrigger;

        private void Start()
        {
            EventSystem.Subscribe<DjmpUsed>(OnDjmpUsed);
            EventSystem.Subscribe<DjmpAvailable>(OnDjmpAvailable);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventSystem.Unsubscribe<DjmpUsed>(OnDjmpUsed);
            EventSystem.Unsubscribe<DjmpAvailable>(OnDjmpAvailable);
        }

        private void OnDjmpAvailable(DjmpAvailable obj)
        {
            animator.SetTrigger(fillTrigger);
        }

        private void OnDjmpUsed(DjmpUsed obj)
        {
            gameObject.SetActive(true);
            animator.SetTrigger(depleteTrigger);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public class DjmpUsed : IEvent
    {
        public void Reset()
        {
        }

        public void Assign(params object[] parameters)
        {
        }
    }

    public class DjmpAvailable : IEvent
    {
        public void Reset()
        {
        }

        public void Assign(params object[] parameters)
        {
        }
    }
}