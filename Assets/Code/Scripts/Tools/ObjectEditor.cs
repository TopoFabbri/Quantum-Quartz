using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Scripts.Tools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class ObjectEditor : Editor
    {
        const int MAX_DEPTH = 10;
        public const BindingFlags ALL_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        readonly Comparer<CustomAttribute> customAttributeComparer = Comparer<CustomAttribute>.Create((a, b) => a.order.CompareTo(b.order));

        public static int Indent { get; private set; }

        static GUIStyle _horizontalLine;
        static GUIStyle HorizontalLine
        {
            get
            {
                if (_horizontalLine == null)
                {
                    _horizontalLine = new GUIStyle();
                    _horizontalLine.margin = new RectOffset(0, 0, 8, 4);
                    _horizontalLine.fixedHeight = 1;
                    _horizontalLine.normal.background = Texture2D.grayTexture;
                }
                return _horizontalLine;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.Box(GUIContent.none, HorizontalLine);

            foreach (Object target in targets)
            {
                DrawInspector(target);
            }
        }

        void DrawInspector(object obj, int depth = 0)
        {
            if (depth > MAX_DEPTH || obj == null) return;

            System.Type type = obj.GetType();
            List<MemberInfo> members = new List<MemberInfo>(type.GetMembers(ALL_FLAGS));
            List<MemberInfo> sortedMembers = members.FindAll((MemberInfo mem) => !(mem is MethodInfo));
            sortedMembers.AddRange(members.FindAll((MemberInfo mem) => (mem is MethodInfo)));

            foreach (MemberInfo member in sortedMembers)
            {
                List<CustomAttribute> attrs = new List<CustomAttribute>(member.GetCustomAttributes<CustomAttribute>(true));
                AttributeProcessing processingFlags = AttributeProcessing.Normal;
                if (attrs.Count > 0)
                {
                    attrs.Sort(customAttributeComparer);
                    foreach (CustomAttribute attr in attrs)
                    {
                        AttributeProcessing? processing = attr.Draw(member, obj);

                        if (processing.HasValue)
                        {
                            processingFlags |= processing.Value;
                            if (processing.Value.HasFlag(AttributeProcessing.None))
                            {
                                break;
                            }
                        }
                    }
                }

                if (processingFlags.HasFlag(AttributeProcessing.Disabled))
                {
                    GUI.enabled = true;
                }
                else
                {
                    FieldInfo field = member as FieldInfo;
                    if (field != null && processingFlags.HasFlag(AttributeProcessing.Expand))
                    {
                        if (obj.GetType().IsAssignableFrom(field.FieldType))
                        {
                            Debug.LogError("Can't expand '" + obj.GetType() + "." + field.Name + "' as it is of type '" + field.FieldType + "', which is a derived class of '" + obj.GetType() + "'");
                        }
                        else
                        {
                            if (processingFlags.HasFlag(AttributeProcessing.Indent))
                            {
                                Indent += 1;
                            }

                            DrawInspector(field.GetValue(obj), depth + 1);
                            Indent -= 1;
                        }
                    }
                }
            }
        }

        public static GUIStyle IndentStyle(GUIStyle style)
        {
            GUIStyle newStyle = new GUIStyle(style);
            newStyle.margin.left += Indent * 45;
            return newStyle;
        }
    }
#endif
}
