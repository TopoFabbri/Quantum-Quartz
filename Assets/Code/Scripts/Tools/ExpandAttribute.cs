using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Code.Scripts.Tools
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ExpandAttribute : CustomAttribute
    {
        readonly HeaderPlusAttribute title;
        public bool DoIndent => title != null;

        public ExpandAttribute()
        {
            order = int.MaxValue;
        }

        public ExpandAttribute(string title)
        {
            this.title = new HeaderPlusAttribute(title);
            this.order = int.MaxValue;
        }

#if UNITY_EDITOR
        public override AttributeProcessing? Draw(MemberInfo target, object obj)
        {
            title?.DrawHeader();
            return AttributeProcessing.Expand | (DoIndent ? AttributeProcessing.Indent : AttributeProcessing.Normal);
        }
#endif
    }
}
