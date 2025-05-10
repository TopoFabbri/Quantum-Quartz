using UnityEngine;
using UnityEditor;
using dninosores.UnityEditorAttributes;


namespace Code.Scripts.Tools.Editor
{
	[CustomPropertyDrawer(typeof(ToggleShow))]
	public class ToggleShowDrawer : PropertyDrawer
	{
		// Necessary since some properties tend to collapse smaller than their content
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return (ShowProperty(property) ? EditorGUI.GetPropertyHeight(property, label, true) : -2);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = true;
			if (ShowProperty(property))
			{
				PropertyDrawer drawer = PropertyDrawerFinder.FindDrawerForProperty(property);
				if (drawer != null)
				{
					drawer.OnGUI(position, property, label);
				}
				else
				{
					EditorGUI.PropertyField(position, property, label, true);
				}
			}
			GUI.enabled = true;
		}

		bool ShowProperty(SerializedProperty property)
		{
			ToggleShow toggle = attribute as ToggleShow;

			// Find the property corresponding to the name provided to the ToggleShow attribute
			SerializedProperty checkboxProperty = property.serializedObject.FindProperty(toggle.checkboxName);

			if (checkboxProperty != null && checkboxProperty.type == "bool")
			{
				bool checkboxValue = checkboxProperty.boolValue;
				return (checkboxValue != toggle.invert);
			}
			else
			{
				Debug.LogError("Error: Boolean '" + toggle.checkboxName + "' in ToggleShow attribute is not valid");
				return false;
			}
		}
	}
}