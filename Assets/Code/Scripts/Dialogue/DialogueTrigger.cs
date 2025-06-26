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
            gameObject.GetComponent<Collider2D>().enabled = false;
            StartConversation();
        }

        private void StartConversation()
        {
            foreach (Conversation.TextBox textBox in conversation.textBoxes)
            {
                Debug.Log(textBox.Text);
            }

            OnConversationEnd();
        }

        private void OnConversationEnd()
        {
            if (reusable)
            {
                gameObject.GetComponent<Collider2D>().enabled = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
