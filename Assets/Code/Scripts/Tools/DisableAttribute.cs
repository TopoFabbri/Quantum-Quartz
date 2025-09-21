using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Scripts.Tools
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class DisableAttribute : CustomAttribute
    {
        readonly bool inEdit = true;
        readonly bool inPlay = true;
        public bool Show => GUI.enabled && (Application.isPlaying ? !inPlay : !inEdit);
        public readonly bool hideValue = false;

        public DisableAttribute(bool inEdit = true, bool inPlay = true, bool hideValue = false)
        {
            this.inEdit = inEdit;
            this.inPlay = inPlay;
            this.hideValue = hideValue;
        }

#if UNITY_EDITOR
        public override AttributeProcessing GetProcessingType(MemberInfo target, object obj)
        {
            return Show ? AttributeProcessing.Normal : AttributeProcessing.Disabled | (hideValue ? AttributeProcessing.None : 0);
        }

        public override void Draw(MemberInfo target, object obj)
        {
            return;
        }

        public override float? TryGetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Show || !hideValue ? null : EditorGUI.GetPropertyHeight(property, label, false);
        }

        public override System.Action OnPreGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (GUI.enabled)
            {
                if (!Show)
                {
                    GUI.enabled = false;
                    return () =>
                    {
                        GUI.enabled = true;
                    };
                }
            }
            return null;
        }

        public override bool DoCustomOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!Show && hideValue)
            {
                EditorGUI.LabelField(position, label);
                return true;
            }
            return false;
        }
#endif
    }
}
