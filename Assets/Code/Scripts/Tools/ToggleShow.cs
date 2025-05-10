using Code.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
public class ToggleShow : CustomAttribute
{
    public string checkboxName;
    public bool invert;

    public ToggleShow(string checkboxName, bool invert = false)
    {
        this.checkboxName = checkboxName;
        this.invert = invert;
        this.order = int.MinValue;
    }

    public override AttributeProcessing? Draw(MemberInfo target, object obj)
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
}
