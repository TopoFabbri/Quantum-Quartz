using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Code.Scripts.Tools.Editor
{
    [CustomPropertyDrawer(typeof(CustomAttribute), true)]
    public class CustomAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float? height = null;
            foreach (CustomAttribute attr in fieldInfo.GetCustomAttributes<CustomAttribute>(true).OrderBy(s => s.order))
            {
                height = attr.TryGetPropertyHeight(property, label);
                if (height.HasValue)
                {
                    break;
                }
            }
            return height ?? EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color backgroundColor = GUI.backgroundColor;
            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            int depth = GUI.depth;
            bool enabled = GUI.enabled;
            Matrix4x4 matrix = GUI.matrix;
            GUISkin skin = GUI.skin;
            string tooltip = GUI.tooltip;

            List<CustomAttribute> attributes = fieldInfo.GetCustomAttributes<CustomAttribute>(true).OrderBy(s => s.order).ToList();
            Stack<System.Action> postGUIStack = new Stack<System.Action>();

            foreach (CustomAttribute attr in attributes)
            {
                postGUIStack.Push(attr.OnPreGUI(position, property, label));
            }

            bool customGUI = false;
            foreach (CustomAttribute attr in attributes)
            {
                if (attr.DoCustomOnGUI(position, property, label))
                {
                    customGUI = true;
                    break;
                }
            }
            if (!customGUI)
            {
                List<PropertyDrawer> drawers = PropertyDrawerFinder.FindDrawersForProperty(property);
                drawers = drawers.Where((d) => d != null && d.GetType() != typeof(CustomAttributeDrawer)).ToList();
                if (drawers.Count > 0)
                {
                    foreach (PropertyDrawer d in drawers)
                    {
                        d.OnGUI(position, property, label);
                    }
                }
                else
                {
                    // Would use EditorGUI.PropertyField, but it has a bug in it, so a custom version without the bug is used instead
                    DrawPropertyFieldReflection.Execute(position, property, label, true);
                }
            }

            foreach (System.Action postGUI in postGUIStack)
            {
                postGUI?.Invoke();
            }

            GUI.backgroundColor = backgroundColor;
            GUI.color = color;
            GUI.contentColor = contentColor;
            GUI.depth = depth;
            GUI.enabled = enabled;
            GUI.matrix = matrix;
            GUI.skin = skin;
            GUI.tooltip = tooltip;
        }
    }
}
