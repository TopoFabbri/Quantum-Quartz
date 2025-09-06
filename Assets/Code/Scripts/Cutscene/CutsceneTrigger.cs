using UnityEngine;

namespace Code.Scripts.Cutscene
{
    public class CutsceneTrigger : MonoBehaviour
    {
        [SerializeField] private string triggerName = "Enter trigger name";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger || !other.CompareTag("Player"))
                return;

            if (!CutsceneManager.HasInstance)
            {
                Debug.LogError("Error: No Cutscene Manager available");
                return;
            }

            CutsceneManager.Instance.TriggerAnimation(triggerName);
        }
    }
}
