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

        private void Start()
        {
            SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
            if (sprite)
                sprite.enabled = false;
            targetHighlight.SetActive(false);
        }

        protected override void OnInteracted()
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            if (reusable)
            {
                DialogueManager.Instance.StartDialogue(conversation, OnConversationEnd);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(conversation, null);
                Destroy(gameObject);
            }
        }

        protected override void OnAwaitingInteraction(bool awaitingInteraction)
        {
            base.OnAwaitingInteraction(awaitingInteraction);
            targetHighlight.SetActive(awaitingInteraction);
        }

        private void OnConversationEnd()
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
            enableAfter?.SetActive(true);
        }
    }
}
