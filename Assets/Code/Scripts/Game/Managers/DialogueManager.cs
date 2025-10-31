using AYellowpaper.SerializedCollections;
using Code.Scripts.Game.Visuals;
using Code.Scripts.Input;
using Code.Scripts.Menu;
using Code.Scripts.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Game.Managers
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        private const float CHAR_DELAY = 0.05f;
        private const float FAST_CHAR_DELAY = 0.0005f;

        public abstract class DialogueTagMapping
        {
            [SerializeField] private string keyword;
            public string Keyword => keyword;
        }

        [System.Serializable]
        public class ButtonMapping : DialogueTagMapping
        {
            [SerializeField] private SerializedDictionary<InputManager.ControlScheme, string> mappings;
            public IReadOnlyDictionary<InputManager.ControlScheme, string> Mappings => mappings;
        }

        [System.Serializable]
        public class SpecialMapping : DialogueTagMapping
        {
            public enum SpecialEffects
            {
                Delay
            }

            [SerializeField] private SerializedDictionary<SpecialEffects, string> mappings;
            public IReadOnlyDictionary<SpecialEffects, string> Mappings => mappings;
        }

        [SerializeField] private DialoguePanelController dialoguePanel;
        [SerializeField] private PauseController pauseController;
        [SerializeField] private SerializedDictionary<Conversation.PortraitAnimation, Conversation.PortraitAlignment> portraitAlignments;
        [SerializeField] private List<ButtonMapping> buttonMappings;
        [SerializeField] private List<SpecialMapping> specialMappings;

        private bool advanceText = false;
        private Coroutine curDialogue = null;
        private Action onCurDialogueEnd = null;

        private void Reset()
        {
            foreach (Conversation.PortraitAnimation anims in Enum.GetValues(typeof(Conversation.PortraitAnimation)))
            {
                portraitAlignments.Add(anims, Conversation.PortraitAlignment.Left);
            }
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

        static readonly Regex WHITESPACE = new Regex(@"\s+", RegexOptions.IgnoreCase);
        static readonly Regex TAG_ATTR_SEPARATION = new Regex(@"(?<!=)\s+(?!=)", RegexOptions.IgnoreCase);
        IEnumerator DialogueLoop(Conversation conversation)
        {
            pauseController.EnterDialogue();
            InputManager.Instance.EnableUIMap();
            GameManager.Instance.SetMusicFaded(true);
            yield return dialoguePanel.Show();

            foreach (Conversation.TextBox textBox in conversation.textBoxes)
            {
                dialoguePanel.SetPortrait(textBox.portrait, portraitAlignments.GetValueOrDefault(textBox.portrait, Conversation.PortraitAlignment.None));

                float wait = 0;
                for (int i = 1; i < textBox.Text.Length; i++)
                {
                    string text = ProcessText(textBox.Text, ref i, ref wait);
                    if (text != null)
                    {
                        dialoguePanel.SetText(text);
                        wait += advanceText ? FAST_CHAR_DELAY : CHAR_DELAY;

                        if (wait >= Time.deltaTime)
                        {
                            AkSoundEngine.PostEvent("Play_DX_Molly_Standard", gameObject);
                            yield return new WaitForSecondsRealtime(wait);
                            wait = 0;
                        }
                    }
                }

                advanceText = false;
                dialoguePanel.FreezePortrait();
                dialoguePanel.SetText(ApplyKeywordMappings(textBox.Text.Replace("\\>", ">").Replace("\\<", "<")));
                yield return new WaitUntil(() => advanceText);
                advanceText = false;
            }

            yield return dialoguePanel.Hide();
            EndDialogue(false);
        }

        private void EndDialogue(bool interrupted)
        {
            if (!interrupted)
            {
                pauseController.ExitDialogue();
                InputManager.Instance.EnableGameMap();
                GameManager.Instance.SetMusicFaded(false);
            }

            onCurDialogueEnd?.Invoke();
            curDialogue = null;
            onCurDialogueEnd = null;
        }

        public void AdvanceText()
        {
            advanceText = true;
        }

        static readonly Regex ANGLE_BRACKETS = new Regex(@"(?<!\\)[><]", RegexOptions.IgnoreCase);
        static readonly string[] ATOMIC_TAGS = new string[] { "sprite", "button", "special", "br" };
        private string WrapTextInTag(string text, string startTag, string endTag)
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
                    if (textParts[i].Length <= 0)
                    {
                        // No need to wrap empty
                        output += endBracket;
                    }
                    else
                    {
                        output += startTag + textParts[i] + endTag + endBracket;
                    }
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
                        // Atomic tags are like regular characters, so they don't need to end the wrapper tag and start a new one
                        if (output.EndsWith(endTag + startBracket))
                        {
                            // The wrapper tag ended before this atomic tag, remove the end to continue it
                            output = output.Substring(0, output.LastIndexOf(endTag + startBracket)) + startBracket;
                        }
                        output += textParts[i] + endBracket + endTag;
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

        private string ApplyKeywordMappings(string text)
        {
            string output = text;
            InputManager.ControlScheme controlScheme = InputManager.GetControlScheme();
            foreach (ButtonMapping buttonMapping in buttonMappings)
            {
                string keyword = buttonMapping.Keyword.Replace("<button=", "").Replace(">", "");
                Regex matchRegex = new Regex(@"<\s*button\s*=\s*" + keyword + ".*?>", RegexOptions.IgnoreCase);
                output = matchRegex.Replace(output, buttonMapping.Mappings[controlScheme]);
            }
            foreach (SpecialMapping specialMapping in specialMappings)
            {
                string keyword = specialMapping.Keyword.Replace("<special=", "").Replace(">", "");
                Regex matchRegex = new Regex(@"<\s*special\s*=\s*" + keyword + ".*?>", RegexOptions.IgnoreCase);
                output = matchRegex.Replace(output, "");
            }
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="i"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public string ProcessText(string text, ref int i, ref float wait)
        {
            if (i >= text.Length)
            {
                return ApplyKeywordMappings(text.Replace("\\>", ">").Replace("\\<", "<"));
            }

            // If empty char, skip
            if (!string.IsNullOrWhiteSpace(text[i].ToString()))
            {
                // Since '\' can be used to escape '>' and '<' symbols, have to skip over the invisible '\'
                if (text[i - 1] == '\\' && (text[i] == '<' || text[i] == '>'))
                {
                    i++;
                }

                // If just processed a '<', skip ahead to next '>'
                if (text[i - 1] == '<')
                {
                    int startBracket = i - 1;
                    // Advance until exceeding text length, or finding a '>' that isn't preceded by a '\'
                    while (i < text.Length && text[i] != '>' && text[i - 1] != '\\')
                    {
                        i++;
                    }

                    // Custom tag handling for "special" tags
                    string tagContent = text.Substring(startBracket + 1, i - startBracket - 1).Trim();
                    if (tagContent.StartsWith("special", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Split the tag's contents into attributes
                        Dictionary<string, string> attributeDict = new Dictionary<string, string>();
                        foreach (string attr in TAG_ATTR_SEPARATION.Split(WHITESPACE.Replace(tagContent, " ")).Where(str => !string.IsNullOrWhiteSpace(str)).ToArray())
                        {
                            string[] attrParts = attr.Split(new[] { '=' }, 2);
                            if (attrParts.Length == 2)
                            {
                                attributeDict.Add(attrParts[0].Trim(), attrParts[1].Trim());
                            }
                        }

                        // Find the entry in specialMappings and apply its effects
                        bool found = false;
                        foreach (SpecialMapping specialMapping in specialMappings)
                        {
                            if (attributeDict.TryGetValue("special", out string keyword) && specialMapping.Keyword.Equals(keyword, StringComparison.InvariantCultureIgnoreCase))
                            {
                                // Found the entry in specialMappings
                                found = true;
                                foreach (SpecialMapping.SpecialEffects attr in specialMapping.Mappings.Keys)
                                {
                                    if (attributeDict.TryGetValue(specialMapping.Mappings[attr], out string value))
                                    {
                                        // Found the effect's attribute on the tag
                                        switch (attr)
                                        {
                                            case SpecialMapping.SpecialEffects.Delay:
                                                if (float.TryParse(value, out float floatValue))
                                                {
                                                    wait += advanceText ? 0 : floatValue;
                                                }
                                                else
                                                {
                                                    Debug.LogError(
                                                        "Error with tag: '" + tagContent + "'"
                                                        + " | " + specialMapping.Mappings[attr] + " is not a float (" + value + ")"
                                                        + " | The '" + attr + "' effect in Special Mappings requires the attribute value to be a valid float"
                                                    );
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogError(
                                            "Error with tag: '" + tagContent + "'"
                                            + " | " + specialMapping.Mappings[attr] + " is missing"
                                            + " | The '" + attr + "' effect of '" + keyword + "' in Special Mappings uses the '" + specialMapping.Mappings[attr] + "' attribute"
                                        );
                                    }
                                }
                                break;
                            }
                        }

                        if (!found)
                        {
                            Debug.LogError("Error with tag: '" + tagContent + "' | Unable to find corresponding entry in Special Mappings for '" + attributeDict.GetValueOrDefault("special", null) + "'");
                        }
                    }
                }
                else
                {
                    return ApplyKeywordMappings((text.Substring(0, i) + WrapTextInTag(text.Substring(i), "<color=#00000000>", "</color>")).Replace("\\>", ">").Replace("\\<", "<"));
                }
            }

            return null;
        }
    }
}
