using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Scripts.Tools.Editor
{
    //https://github.com/Unity-Technologies/UnityCsReference/blob/d6f29af28d9f82f07d2a29dc8484458adf861486/Editor/Mono/Inspector/Core/ScriptAttributeGUI/Implementations/DecoratorDrawers.cs#L34
    [CustomPropertyDrawer(typeof(HeaderPlusAttribute))]
    public class HeaderPlusDrawer : DecoratorDrawer
    {
        public const string headerLabelClassName = "custom-header-plus-drawer__label";

        public override void OnGUI(Rect position)
        {
            position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
            position = EditorGUI.IndentedRect(position);
            GUI.Label(position, (attribute as HeaderPlusAttribute).header, EditorStyles.boldLabel);
        }

        public override float GetHeight()
        {
            float fullTextHeight = EditorStyles.boldLabel.CalcHeight(new GUIContent((attribute as HeaderPlusAttribute).header), 1.0f);
            int lines = 1;
            if ((attribute as HeaderPlusAttribute).header != null)
            {
                lines = (attribute as HeaderPlusAttribute).header.Count(a => a == '\n') + 1;
            }
            float eachLineHeight = fullTextHeight / lines;
            return EditorGUIUtility.singleLineHeight * 1.5f + (eachLineHeight * (lines - 1));
        }

        public override VisualElement CreatePropertyGUI()
        {
            string text = (attribute as HeaderPlusAttribute).header;
            Label label = new Label(text);

            label.AddToClassList(headerLabelClassName);

            return label;
        }
    }
}
