using Code.Scripts.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Dialogue
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        [SerializeField] private Animator dialoguePanelAnim;
        [SerializeField] private TextMeshProUGUI dialogueText;

        public bool skipText = false;
        private Coroutine curDialogue = null;
        private Action onCurDialogueEnd = null;

        public void StartDialogue(Conversation conversation, Action onConversationEnd = null)
        {
            if (curDialogue != null)
            {
                StopCoroutine(curDialogue);
                EndDialogue();
            }

            curDialogue = StartCoroutine(DialogueLoop(conversation));
            onCurDialogueEnd = onConversationEnd;
        }

        IEnumerator DialogueLoop(Conversation conversation)
        {
            dialoguePanelAnim.gameObject.SetActive(true);

            foreach (Conversation.TextBox textBox in conversation.textBoxes)
            {
                dialogueText.text = textBox.Text;
                dialoguePanelAnim.SetInteger("Portrait", (int)textBox.portrait);
                yield return new WaitUntil(() => skipText);
                skipText = false;
            }

            EndDialogue();
        }

        private void EndDialogue()
        {
            dialoguePanelAnim.gameObject.SetActive(false);
            dialogueText.text = "";
            dialoguePanelAnim.SetInteger("Portrait", (int)Conversation.PortraitAnimation.None);

            onCurDialogueEnd?.Invoke();
            curDialogue = null;
            onCurDialogueEnd = null;
        }
    }
}
