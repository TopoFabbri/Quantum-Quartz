using Code.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Dialogue
{
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private bool reusable = false;
        [SerializeField] private Conversation conversation;

        private void Start()
        {
            SpriteRenderer a = gameObject.GetComponent<SpriteRenderer>();
            if (a)
                a.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out PlayerController _))
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
        }

        private void OnConversationEnd()
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }
}
