using Code.Scripts.Level;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Code.Scripts.Tools
{
    public abstract class CustomAttribute : System.Attribute
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matching Unity's built-in PropertyAttribute's order field")]
        public int order { get; set; }

#if UNITY_EDITOR
        public abstract void Draw(MemberInfo target);
#endif
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ExpandAttribute : System.Attribute
    {
        readonly HeaderPlusAttribute title;

        public ExpandAttribute() { }

        public ExpandAttribute(string title)
        {
            this.title = new HeaderPlusAttribute(title);
        }

        public bool Draw(MemberInfo target)
        {
            title?.Draw(target);
            return title != null;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class ObjectEditor : Editor
    {
        const int MAX_DEPTH = 10;
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
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
                DrawInspector(target.GetType());
            }
        }

        void DrawInspector(System.Type type, int depth = 0)
        {
            if (depth > MAX_DEPTH) return;

            List<MemberInfo> members = new List<MemberInfo>(type.GetMembers(flags));
            List<MemberInfo> sortedMembers = members.FindAll((MemberInfo mem) => !(mem is MethodInfo));
            sortedMembers.AddRange(members.FindAll((MemberInfo mem) => (mem is MethodInfo)));

            foreach (MemberInfo member in sortedMembers)
            {
                List<CustomAttribute> attrs = new List<CustomAttribute>(member.GetCustomAttributes<CustomAttribute>(true));
                if (attrs.Count > 0)
                {
                    attrs.Sort(customAttributeComparer);
                    foreach (CustomAttribute attr in attrs)
                    {
                        attr.Draw(member);
                    }
                }

                FieldInfo field = member as FieldInfo;
                ExpandAttribute expandAttr = member.GetCustomAttribute<ExpandAttribute>();
                if (field != null && expandAttr != null)
                {
                    if (expandAttr.Draw(member))
                    {
                        Indent += 1;
                    }
                    DrawInspector(field.FieldType, depth + 1);
                    Indent -= 1;
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
