using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Scripts.Tools
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class ToggleShowAttribute : CustomAttribute
    {
        public string checkboxName;
        public bool invert;

        public ToggleShowAttribute(string checkboxName, bool invert = false)
        {
            this.checkboxName = checkboxName;
            this.invert = invert;
            this.order = int.MinValue;
        }

#if UNITY_EDITOR
        public override AttributeProcessing GetProcessingType(MemberInfo target, object obj)
        {
            bool show = false;
            // Find the field corresponding to the name provided to the ToggleShow attribute
            FieldInfo field = target.DeclaringType.GetField(checkboxName, ObjectEditor.ALL_FLAGS);

            if (field != null && field.FieldType == typeof(bool))
            {
                bool checkboxValue = (bool)field.GetValue(obj);
                show = (checkboxValue != invert);
            }
            else
            {
                Debug.LogError("Error: Boolean '" + checkboxName + "' in ToggleShow attribute is not valid");
                show = false;
            }

            return show ? AttributeProcessing.Normal : AttributeProcessing.None;
        }

        public override void Draw(MemberInfo target, object obj)
        {
            return;
        }

        public override float? TryGetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (ShowProperty(property) ? null : -2);
        }

        public override bool DoCustomOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            return !ShowProperty(property);
        }

        bool ShowProperty(SerializedProperty property)
        {
            // Find the property corresponding to the name provided to the ToggleShow attribute
            SerializedProperty checkboxProperty = property.FindParentProperty()?.FindPropertyRelative(checkboxName) ?? property.serializedObject.FindProperty(checkboxName);

            if (checkboxProperty != null && checkboxProperty.type == "bool")
            {
                bool checkboxValue = checkboxProperty.boolValue;
                return (checkboxValue != invert);
            }
            else
            {
                Debug.LogError("Error: Boolean '" + checkboxName + "' in ToggleShow attribute is not valid");
                return false;
            }
        }
#endif
    }
}
