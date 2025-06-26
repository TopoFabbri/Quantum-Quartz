using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Scripts.Dialogue
{
    [CreateAssetMenu(menuName = "Custom/Conversation", fileName = "Conversation", order = 1)]
    public class Conversation : ScriptableObject
    {
        public enum PortraitAnimation
        {
            None,
            Molly_Normal,
            Molly_Happy,
            Molly_Sad
        }

        [System.Serializable]
        public struct TextBox
        {
            [SerializeField] private LocalizedString text;
            public PortraitAnimation portrait;
            public string Text => text.GetLocalizedString();
        }

        public List<TextBox> textBoxes;
    }
}
