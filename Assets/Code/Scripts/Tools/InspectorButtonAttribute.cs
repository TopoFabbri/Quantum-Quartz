using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace Code.Scripts.Tools
{
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class InspectorButtonAttribute : CustomAttribute
    {
        public string ButtonText { get; private set; }

        public InspectorButtonAttribute(string text)
        {
            ButtonText = text;
        }

#if UNITY_EDITOR
        public override void Draw(MemberInfo target, object obj)
        {
            MethodInfo method = target as MethodInfo;
            if (method == null)
                return;

            if (method.GetParameters().Length > 0)
            {
                throw new TargetParameterCountException("InspectorButton attribute can not be used on methods with parameters.");
            }
            else
            {
                if (GUILayout.Button(ButtonText, ObjectEditor.IndentStyle(EditorStyles.miniButton)))
                {
                    method.Invoke(target, null);
                }
            }

            return;
        }
#endif
    }
}
