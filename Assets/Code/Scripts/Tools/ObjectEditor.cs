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
            // Limit recursion depth
            if (depth > MAX_DEPTH || obj == null) return;

            bool guiEnabled = GUI.enabled;

            // Get all members for the given object's type, then sort them so that methods go after properties/fields
            System.Type type = obj.GetType();
            List<MemberInfo> members = new List<MemberInfo>(type.GetMembers(ALL_FLAGS));
            List<MemberInfo> sortedMembers = members.FindAll((MemberInfo mem) => !(mem is MethodInfo));
            sortedMembers.AddRange(members.FindAll((MemberInfo mem) => (mem is MethodInfo)));

            // Get the necessary data from all the members
            List<(MemberInfo, List<CustomAttribute>, AttributeProcessing)> memberData = new List<(MemberInfo, List<CustomAttribute>, AttributeProcessing)>();
            foreach (MemberInfo member in sortedMembers)
            {
                // Get all the member's CustomAttributes
                List<CustomAttribute> attrs = new List<CustomAttribute>(member.GetCustomAttributes<CustomAttribute>(true));
                if (attrs.Count > 0)
                {
                    // Sort the attributes by their order values
                    attrs.Sort(customAttributeComparer);

                    // Get all the processing flags related to the member
                    AttributeProcessing processingFlags = AttributeProcessing.Normal;
                    foreach (CustomAttribute attr in attrs)
                    {
                        processingFlags |= attr.GetProcessingType(member, obj);
                    }

                    memberData.Add((member, attrs, processingFlags));
                }
            }

            // Process the member data
            List<(MemberInfo, List<CustomAttribute>, AttributeProcessing)> lateProcessingData = new List<(MemberInfo, List<CustomAttribute>, AttributeProcessing)>();
            foreach (var memberAttrsFlags in memberData)
            {
                MemberInfo member = memberAttrsFlags.Item1;
                List<CustomAttribute> attrs = memberAttrsFlags.Item2;
                AttributeProcessing processingFlags = memberAttrsFlags.Item3;

                // If the member has a None flag, don't process it
                // If it has an Expand flag, delay its processing to the end
                // If it has a Disabled flag, set GUI.enabled to false, disabling any fields that get drawn
                if (processingFlags.HasFlag(AttributeProcessing.None))
                {
                    continue;
                }
                else if (processingFlags.HasFlag(AttributeProcessing.Expand))
                {
                    lateProcessingData.Add(memberAttrsFlags);
                    continue;
                }
                else if (processingFlags.HasFlag(AttributeProcessing.Disabled))
                {
                    GUI.enabled = false;
                }

                // Draw all the member's attributes
                foreach (CustomAttribute attr in attrs)
                {
                    attr.Draw(member, obj);
                }

                // Return GUI.enabled to its prior state
                GUI.enabled = guiEnabled;
            }

            // Process the late processing member data
            foreach (var memberAttrsFlags in lateProcessingData)
            {
                MemberInfo member = memberAttrsFlags.Item1;
                List<CustomAttribute> attrs = memberAttrsFlags.Item2;
                AttributeProcessing processingFlags = memberAttrsFlags.Item3;

                // If the member has a Disabled flag, set GUI.enabled to false, disabling any fields that get drawn
                if (processingFlags.HasFlag(AttributeProcessing.Disabled))
                {
                    GUI.enabled = false;
                }

                // Draw all the member's attributes
                foreach (CustomAttribute attr in attrs)
                {
                    attr.Draw(member, obj);
                }

                // If the member has an Expand flag (field only), process its child members as well
                FieldInfo field = member as FieldInfo;
                if (field != null && processingFlags.HasFlag(AttributeProcessing.Expand))
                {
                    // Prevent processing of child members that match the type of the parent, or that are of a type derived from the parent (Prevents loops)
                    if (obj.GetType().IsAssignableFrom(field.FieldType))
                    {
                        Debug.LogError("Can't expand '" + obj.GetType() + "." + field.Name + "' as it is of type '" + field.FieldType + "', which is a derived class of '" + obj.GetType() + "'");
                    }
                    else
                    {
                        // If the member has an Indent flag, indent the child members
                        if (processingFlags.HasFlag(AttributeProcessing.Indent))
                        {
                            Indent += 1;
                        }

                        // Draw the child members
                        DrawInspector(field.GetValue(obj), depth + 1);
                        Indent -= 1;
                    }
                }

                // Return GUI.enabled to its prior state
                GUI.enabled = guiEnabled;
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
