using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Scripts.Tools;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace Code.Scripts.Tools
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class HeaderPlusAttribute : CustomAttribute
    {
        public string header;
        static GUIStyle _style;
        static GUIStyle Style
        {
            get
            {
                if (_style == null)
                {
                    _style = new GUIStyle(EditorStyles.boldLabel);
                }
                return _style;
            }
        }

        public HeaderPlusAttribute(string header)
        {
            this.header = header;
        }

#if UNITY_EDITOR
        public override void Draw(MemberInfo target)
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f + 2); // No clue why I need to add 2 to match Unity's headers
            GUILayout.Label(header, ObjectEditor.IndentStyle(Style));
        }
#endif
    }
}