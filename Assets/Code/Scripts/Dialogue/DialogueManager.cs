using Code.Scripts.Input;
using Code.Scripts.Tools;
using Code.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Dialogue
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        private const float CHAR_DELAY = 0.05f;
        private const float FAST_CHAR_DELAY = 0.001f;

        [SerializeField] private Animator dialoguePanelAnim;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button dialogueButton;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private PauseController pauseController;
        [SerializeField] private Event pauseEvent;
        [SerializeField] private Event unPauseEvent;

        private bool advanceText = false;
        private Coroutine curDialogue = null;
        private Action onCurDialogueEnd = null;

        public void StartDialogue(Conversation conversation, Action onConversationEnd = null)
        {
            if (curDialogue != null)
            {
                StopCoroutine(curDialogue);
                EndDialogue(true);
            }

            curDialogue = StartCoroutine(DialogueLoop(conversation));
            onCurDialogueEnd = onConversationEnd;
        }

        IEnumerator DialogueLoop(Conversation conversation)
        {
            pauseController.EnterDialogue();
            inputManager.EnableUIMap();
            dialoguePanelAnim.gameObject.SetActive(true);
            dialogueButton.Select();
            pauseEvent.Post(gameObject);
            Time.timeScale = 0;

            foreach (Conversation.TextBox textBox in conversation.textBoxes)
            {
                dialoguePanelAnim.speed = 1;
                dialoguePanelAnim.SetInteger("Portrait", (int)textBox.portrait);

                for (int i = 1; i < textBox.Text.Length; i++)
                {
                    // If empty char, skip
                    if (!string.IsNullOrWhiteSpace(textBox.Text[i].ToString()))
                    {
                        // Since '\' can be used to escape '>' and '<' symbols, have to skip over the invisible '\'
                        if (textBox.Text[i - 1] == '\\' && (textBox.Text[i] == '<' || textBox.Text[i] == '>'))
                        {
                            i++;
                        }

                        // If just processed a '<', skip ahead to next '>'
                        if (textBox.Text[i - 1] == '<')
                        {
                            // Advance until exceeding text length, or finding a '>' that isn't preceded by a '\'
                            while (i < textBox.Text.Length && textBox.Text[i] != '>' && textBox.Text[i - 1] != '\\')
                            {
                                i++;
                            }
                        }
                        else
                        {
                            dialogueText.text = textBox.Text.Insert(i, "<color=#00000000>").Replace("\\>", ">").Replace("\\<", "<") + "</color>";
                            yield return new WaitForSecondsRealtime(advanceText ? FAST_CHAR_DELAY : CHAR_DELAY);
                        }
                    }
                }

                advanceText = false;
                dialoguePanelAnim.speed = 0;
                for (int i = 0; i < dialoguePanelAnim.layerCount; i++)
                {
                    dialoguePanelAnim.Play(dialoguePanelAnim.GetCurrentAnimatorStateInfo(i).shortNameHash, i, 0);
                }
                dialogueText.text = textBox.Text.Replace("\\>", ">").Replace("\\<", "<");
                yield return new WaitUntil(() => advanceText);
                advanceText = false;
            }

            EndDialogue(false);
        }

        private void EndDialogue(bool interrupted)
        {
            if (!interrupted)
            {
                pauseController.ExitDialogue();
                inputManager.EnableGameMap();
                dialoguePanelAnim.gameObject.SetActive(false);
                unPauseEvent.Post(gameObject);
                Time.timeScale = 1;
            }
            dialogueText.text = "";
            dialoguePanelAnim.SetInteger("Portrait", (int)Conversation.PortraitAnimation.None);

            onCurDialogueEnd?.Invoke();
            curDialogue = null;
            onCurDialogueEnd = null;
        }

        public void AdvanceText()
        {
            advanceText = true;
        }
    }
}
