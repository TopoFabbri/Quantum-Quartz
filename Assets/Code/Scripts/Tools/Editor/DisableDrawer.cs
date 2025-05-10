using UnityEngine;
using UnityEditor;
using dninosores.UnityEditorAttributes;


namespace Code.Scripts.Tools.Editor
{
	[CustomPropertyDrawer(typeof(DisableAttribute))]
	public class DisableDrawer : PropertyDrawer
	{
		// Necessary since some properties tend to collapse smaller than their content
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, (attribute as DisableAttribute).Show);
		}

		// Draw a disabled property field
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			DisableAttribute attr = (attribute as DisableAttribute);
			GUI.enabled = attr.Show;
			if (attr.Show || !attr.hideValue)
			{
				PropertyDrawer drawer = PropertyDrawerFinder.FindDrawerForProperty(property);
				if (drawer != null)
				{
					drawer.OnGUI(position, property, label);
				}
				else
				{
					if (property.hasVisibleChildren)
					{
						EditorGUI.indentLevel--;
						EditorGUI.PropertyField(position, property, label, true);
						EditorGUI.indentLevel++;
					}
					else
					{
						EditorGUI.PropertyField(position, property, label, true);
					}
				}
			}
			else
			{
				EditorGUI.LabelField(position, label);
			}
			GUI.enabled = true;
		}
	}
}