using UnityEngine;

namespace Code.Scripts.Cutscene
{
    public class CutsceneTrigger : MonoBehaviour
    {
        [SerializeField] private Animator targetAnimator;
        [SerializeField] private string triggerName = "Enter trigger name";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger || !other.CompareTag("Player") || !targetAnimator)
                return;
            
            targetAnimator.SetTrigger(triggerName);
        }
    }
}
