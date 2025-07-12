using Code.Scripts.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Dialogue
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
            SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
            if (sprite)
                sprite.enabled = false;
            targetHighlight.SetActive(false);
        }

        protected override void OnInteracted()
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            DialogueManager.Instance.StartDialogue(conversation, OnConversationEnd);
        }

        protected override void OnAwaitingInteraction(bool awaitingInteraction)
        {
            base.OnAwaitingInteraction(awaitingInteraction);
            targetHighlight.SetActive(awaitingInteraction);
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
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
