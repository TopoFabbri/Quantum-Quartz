using Code.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
public class DisableAttribute : CustomAttribute
{
    readonly bool inEdit = true;
    readonly bool inPlay = true;
    public bool Show => Application.isPlaying ? !inPlay : !inEdit;
    public readonly bool hideValue = true;

    public DisableAttribute(bool inEdit = true, bool inPlay = true, bool hideValue = false)
    {
        this.inEdit = inEdit;
        this.inPlay = inPlay;
        this.hideValue = hideValue;
        this.order = int.MinValue + 1;
    }

#if UNITY_EDITOR
    public override AttributeProcessing? Draw(MemberInfo target, object obj)
    {
        GUI.enabled = Show;
        return Show ? AttributeProcessing.Normal : AttributeProcessing.Disabled | (hideValue ? AttributeProcessing.None : 0);
    }
#endif
}
