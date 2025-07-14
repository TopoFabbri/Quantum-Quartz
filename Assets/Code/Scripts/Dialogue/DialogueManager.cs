using Code.Scripts.Input;
using Code.Scripts.Tools;
using Code.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Dialogue
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        private const float CHAR_DELAY = 0.05f;
        private const float FAST_CHAR_DELAY = 0.0005f;

        [SerializeField] private float portraitOffset = 352;
        [SerializeField] private Animator dialoguePanelAnim;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button dialogueButton;
        [SerializeField] private RectTransform textBox;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private PauseController pauseController;
        [SerializeField] private Event pauseEvent;
        [SerializeField] private Event unPauseEvent;

        private bool advanceText = false;
        private Coroutine curDialogue = null;
        private Action onCurDialogueEnd = null;
        private Rect defaultTextBoxRect;

        private void Awake()
        {
            defaultTextBoxRect = new Rect();
            defaultTextBoxRect.min = textBox.offsetMin;
            defaultTextBoxRect.max = textBox.offsetMax;
        }

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

            foreach (Conversation.TextBox textBox in conversation.textBoxes)
            {
                dialoguePanelAnim.speed = 1;
                this.textBox.offsetMin = defaultTextBoxRect.min + Vector2.right * (textBox.portraitAlignment.HasFlag(Conversation.PortraitAlignment.Left) ? portraitOffset : 0);
                this.textBox.offsetMax = defaultTextBoxRect.max + Vector2.left * (textBox.portraitAlignment.HasFlag(Conversation.PortraitAlignment.Right) ? portraitOffset : 0);
                dialoguePanelAnim.SetInteger("Portrait", (int)textBox.portrait);

                float wait = 0;
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
                            dialogueText.text = (textBox.Text.Substring(0, i) + WrapTextInTag(textBox.Text.Substring(i), "<color=#00000000>", "</color>")).Replace("\\>", ">").Replace("\\<", "<");
                            float delay = advanceText ? FAST_CHAR_DELAY : CHAR_DELAY;
                            if (delay >= Time.deltaTime || wait >= Time.deltaTime)
                            {
                                wait = Mathf.Max(0, wait - Time.deltaTime);
                                yield return new WaitForSecondsRealtime(delay);
                            }
                            else
                            {
                                wait += delay;
                            }
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

        static readonly Regex ANGLE_BRACKETS = new Regex(@"(?<!\\)[><]");
        static readonly string[] ATOMIC_TAGS = new string[] { "sprite" };
        string WrapTextInTag(string text, string startTag, string endTag)
        {
            string output = "";
            string[] textParts = ANGLE_BRACKETS.Split(text);
            int textIdx = 0;
            for (int i = 0; i < textParts.Length; i++)
            {
                int idx = text.IndexOf(textParts[i], textIdx);
                string startBracket = idx == 0 ? "" : text[idx - 1].ToString();
                string endBracket = (idx + textParts[i].Length) >= text.Length ? "" : text[idx + textParts[i].Length].ToString();

                // startBracket is '>' or a non-bracket, and endBracket is '<' or a non-bracket
                if ((startBracket == ">" || startBracket != "<") && (endBracket == "<" || endBracket != ">"))
                {
                    output += startTag + textParts[i] + endTag + endBracket;
                }
                else
                {
                    if (startBracket != "<" || endBracket != ">")
                    {
                        Debug.LogError(
                            "Bracket Error in Text: " + text
                            + " | StartBracket: " + startBracket + " (" + (startBracket.Length > 0 ? (short)startBracket[0] : null) + ")"
                            + " | EndBracket: " + endBracket + " (" + (endBracket.Length > 0 ? (short)endBracket[0] : null) + ")"
                        );
                        break;
                    }
                    else if (ATOMIC_TAGS.Any((tag) => textParts[i].TrimStart().StartsWith(tag, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        output = (output.Length == 0 ? "" : output.Remove(output.Length - 1)) + startTag + startBracket + textParts[i] + endBracket + endTag;
                    }
                    else
                    {
                        output += textParts[i] + endBracket;
                    }
                }

                textIdx += textParts[i].Length + 1;
            }
            return output;
        }
    }
}
