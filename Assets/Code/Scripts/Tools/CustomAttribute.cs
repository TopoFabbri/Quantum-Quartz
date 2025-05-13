using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Scripts.Tools
{
    [System.Flags]
    public enum AttributeProcessing
    {
        Normal = 0,
        None = 1 << 0,
        Disabled = 1 << 1,
        Indent = 1 << 2,
        Expand = 1 << 3,
    }

    public abstract class CustomAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public virtual AttributeProcessing GetProcessingType(MemberInfo target, object obj)
        {
            return AttributeProcessing.Normal;
        }

        public abstract void Draw(MemberInfo target, object obj);

        public virtual float? TryGetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return null;
        }

        public virtual System.Action OnPreGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            return null;
        }

        public virtual bool DoCustomOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            return false;
        }
#endif
    }
}
