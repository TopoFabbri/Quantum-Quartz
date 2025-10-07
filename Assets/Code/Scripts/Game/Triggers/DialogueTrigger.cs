using Code.Scripts.Game.Managers;
using UnityEngine;

namespace Code.Scripts.Game.Triggers
{
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : InteractableComponent
    {
        [SerializeField] private bool interactable = false;
        [SerializeField] private bool reusable = false;
        [SerializeField] private Conversation conversation;
        [SerializeField] private GameObject enableAfter;
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
            gameObject.GetComponent<Collider2D>().enabled = false;
            DialogueManager.Instance.StartDialogue(conversation, OnConversationEnd);
        }

        protected override void OnAwaitingInteraction(bool awaitingInteraction)
        {
            base.OnAwaitingInteraction(awaitingInteraction);
            if (targetHighlight)
            {
                targetHighlight.SetActive(awaitingInteraction);
            }
        }

        private void OnConversationEnd()
        {
            if (enableAfter)
            {
                enableAfter.SetActive(true);
            }
            if (reusable)
            {
                gameObject.GetComponent<Collider2D>().enabled = true;
            }
            else
            {
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
