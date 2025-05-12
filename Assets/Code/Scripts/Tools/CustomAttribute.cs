using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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
        public abstract AttributeProcessing? Draw(MemberInfo target, object obj);
#endif
    }
}
