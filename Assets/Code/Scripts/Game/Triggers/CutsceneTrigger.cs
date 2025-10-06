using Code.Scripts.Game.Managers;
using UnityEngine;

namespace Code.Scripts.Game.Triggers
{
    public class CutsceneTrigger : InteractableComponent
    {
        [SerializeField] private bool interactable = false;
        [SerializeField] private string triggerName = "Enter trigger name";
        [SerializeField] private bool reusable = false;
        [SerializeField] private GameObject targetHighlight;

        public override bool RequiresClick => interactable;

        private void Awake()
        {
            foreach (SpriteRenderer sprite in gameObject.GetComponents<SpriteRenderer>())
            {
                sprite.enabled = false;
            }
            if (targetHighlight)
            {
                targetHighlight.SetActive(false);
            }
        }

        protected override void OnInteracted()
        {
            if (!CutsceneManager.HasInstance)
            {
                Debug.LogError("Error: No Cutscene Manager available");
                return;
            }

            CutsceneManager.Instance.TriggerAnimation(triggerName);

            if (!reusable)
            {
                gameObject.SetActive(false);
            }
        }

        protected override void OnAwaitingInteraction(bool awaitingInteraction)
        {
            base.OnAwaitingInteraction(awaitingInteraction);
            if (targetHighlight)
            {
                targetHighlight.SetActive(awaitingInteraction);
            }
        }
    }
}
