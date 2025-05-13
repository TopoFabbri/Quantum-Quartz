using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Code.Scripts.Tools.Editor
{
    class DrawPropertyFieldReflection
    {
        static DrawPropertyFieldReflection _instance;
        public static DrawPropertyFieldReflection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DrawPropertyFieldReflection();
                }
                return _instance;
            }
        }

        readonly MethodInfo BeginIsInsideList;
        readonly MethodInfo EndIsInsideList;
        readonly MethodInfo GetInsideListDepth;
        readonly MethodInfo DefaultPropertyField;
        readonly MethodInfo HasVisibleChildFields;
        readonly MethodInfo GetSinglePropertyHeight;
        readonly MethodInfo TempContent;
        readonly MethodInfo OnGUISafe;
        readonly PropertyInfo isInsideList;

        readonly System.Type ScriptAttributeUtility;
        readonly MethodInfo GetHandler;

        readonly System.Type PropertyHandler;
        readonly MethodInfo UseReorderabelListControl;
        readonly MethodInfo GetHeight;
        readonly MethodInfo OnGUI;
        readonly MethodInfo TestInvalidateCache;
        readonly MethodInfo IncrementNestingContext;
        readonly PropertyInfo isCurrentlyNested;
        readonly PropertyInfo skipDecoratorDrawers;
        readonly PropertyInfo propertyDrawer;
        readonly FieldInfo s_reorderableLists;
        readonly FieldInfo m_DecoratorDrawers;
        readonly FieldInfo tooltip;
        readonly FieldInfo localizedDisplayName;

        readonly System.Type ReorderableListWrapper;
        readonly MethodInfo GetPropertyIdentifier;
        readonly MethodInfo Draw;
        readonly FieldInfo Property;

        readonly System.Type GUIView;
        readonly PropertyInfo current;
        readonly PropertyInfo screenPosition;

        public DrawPropertyFieldReflection()
        {
            BeginIsInsideList = typeof(EditorGUI).GetMethod("BeginIsInsideList", BindingFlags.NonPublic | BindingFlags.Static);
            EndIsInsideList = typeof(EditorGUI).GetMethod("EndIsInsideList", BindingFlags.NonPublic | BindingFlags.Static);
            GetInsideListDepth = typeof(EditorGUI).GetMethod("GetInsideListDepth", BindingFlags.NonPublic | BindingFlags.Static);
            DefaultPropertyField = typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.NonPublic | BindingFlags.Static);
            HasVisibleChildFields = typeof(EditorGUI).GetMethod("HasVisibleChildFields", BindingFlags.NonPublic | BindingFlags.Static);
            GetSinglePropertyHeight = typeof(EditorGUI).GetMethod("GetSinglePropertyHeight", BindingFlags.NonPublic | BindingFlags.Static);
            TempContent = typeof(EditorGUIUtility).GetMethod("TempContent", BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string), typeof(string) }, null);
            OnGUISafe = typeof(PropertyDrawer).GetMethod("OnGUISafe", BindingFlags.NonPublic | BindingFlags.Instance);
            isInsideList = typeof(GUI).GetProperty("isInsideList", BindingFlags.NonPublic | BindingFlags.Static);

            ScriptAttributeUtility = System.Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor.CoreModule", true, true);
            GetHandler = ScriptAttributeUtility.GetMethod("GetHandler", BindingFlags.NonPublic | BindingFlags.Static);

            PropertyHandler = System.Type.GetType("UnityEditor.PropertyHandler, UnityEditor.CoreModule", true, true);
            UseReorderabelListControl = PropertyHandler.GetMethod("UseReorderabelListControl", BindingFlags.NonPublic | BindingFlags.Static);
            GetHeight = PropertyHandler.GetMethod("GetHeight", BindingFlags.Public | BindingFlags.Instance);
            OnGUI = PropertyHandler.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Instance);
            TestInvalidateCache = PropertyHandler.GetMethod("TestInvalidateCache", BindingFlags.NonPublic | BindingFlags.Instance);
            IncrementNestingContext = PropertyHandler.GetMethod("IncrementNestingContext", BindingFlags.Public | BindingFlags.Instance);
            isCurrentlyNested = PropertyHandler.GetProperty("isCurrentlyNested", BindingFlags.NonPublic | BindingFlags.Instance);
            skipDecoratorDrawers = PropertyHandler.GetProperty("skipDecoratorDrawers", BindingFlags.NonPublic | BindingFlags.Instance);
            propertyDrawer = PropertyHandler.GetProperty("propertyDrawer", BindingFlags.NonPublic | BindingFlags.Instance);
            s_reorderableLists = PropertyHandler.GetField("s_reorderableLists", BindingFlags.NonPublic | BindingFlags.Static);
            m_DecoratorDrawers = PropertyHandler.GetField("m_DecoratorDrawers", BindingFlags.NonPublic | BindingFlags.Instance);
            tooltip = PropertyHandler.GetField("tooltip", BindingFlags.Public | BindingFlags.Instance);
            localizedDisplayName = PropertyHandler.GetField("localizedDisplayName", BindingFlags.NonPublic | BindingFlags.Instance);

            ReorderableListWrapper = System.Type.GetType("UnityEditorInternal.ReorderableListWrapper, UnityEditor.CoreModule", true, true);
            GetPropertyIdentifier = ReorderableListWrapper.GetMethod("GetPropertyIdentifier", BindingFlags.Public | BindingFlags.Static);
            Draw = ReorderableListWrapper.GetMethod("Draw", BindingFlags.Public | BindingFlags.Instance);
            Property = ReorderableListWrapper.GetField("Property", BindingFlags.NonPublic | BindingFlags.Instance);

            GUIView = System.Type.GetType("UnityEditor.GUIView, UnityEditor.CoreModule", true, true);
            current = GUIView.GetProperty("current", BindingFlags.Public | BindingFlags.Static);
            screenPosition = GUIView.GetProperty("screenPosition", BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// This is a recreation of EditorGUI.PropertyField based off Unity's GitHub using reflection.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property field.</param>
        /// <param name="property">The SerializedProperty to make a field for.</param>
        /// <param name="label">Optional label to use. If not specified the label of the property itself is used.<br></br>Use GUIContent.none to not display a label at all.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <returns>True if the property has children and is expanded and includeChildren was set to false; otherwise false.</returns>
        public static bool Execute(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return Instance.PropertyField(position, property, label, includeChildren);
        }

        // 6000.2.0a8: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/EditorGUI.cs#L9043
        /// <summary>
        /// This is a recreation of EditorGUI.PropertyField based off Unity's GitHub using reflection.<br></br>
        /// Unsure why, but the currently implemented version has a bug when it's told to draw a nested property with children, it indents those children as if the nested property where a top level one (double indent).
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property field.</param>
        /// <param name="property">The SerializedProperty to make a field for.</param>
        /// <param name="label">Optional label to use. If not specified the label of the property itself is used.<br></br>Use GUIContent.none to not display a label at all.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <returns>True if the property has children and is expanded and includeChildren was set to false; otherwise false.</returns>
        bool PropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            object handler = GetHandler.Invoke(null, new object[] { property });
            return CustomOnGUI(handler, position, property, label, includeChildren, new Rect(0, 0, float.MaxValue, float.MaxValue));
        }

        // 6000.2.0a9: https://github.com/Unity-Technologies/UnityCsReference/blob/d6f29af28d9f82f07d2a29dc8484458adf861486/Editor/Mono/Inspector/Core/ScriptAttributeGUI/PropertyHandler.cs#L210
        // 2022.3.13f1: https://github.com/Unity-Technologies/UnityCsReference/blob/a0998d79e28f26e61727a6e66f9ba28b874307f8/Editor/Mono/ScriptAttributeGUI/PropertyHandler.cs#L149
        /// <summary>
        /// This is a recreation of PropertyHandler.OnGUI based off Unity's GitHub using reflection.<br></br>
        /// Unsure why, but the currently implemented version has a bug when it's told to draw a nested property with children, it indents those children as if the nested property where a top level one (double indent).
        /// </summary>
        /// <param name="thisHandler">The PropertyHandler instance</param>
        /// <param name="position">Rectangle on the screen to use for the property field.</param>
        /// <param name="property">The SerializedProperty to make a field for.</param>
        /// <param name="label">Optional label to use. If not specified the label of the property itself is used.<br></br>Use GUIContent.none to not display a label at all.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <returns>True if the property has children and is expanded and includeChildren was set to false; otherwise false.</returns>
        bool CustomOnGUI(object thisHandler, Rect position, SerializedProperty property, GUIContent label, bool includeChildren, Rect visibleArea)
        {
            TestInvalidateCache.Invoke(thisHandler, null);

            float oldLabelWidth, oldFieldWidth;

            float propHeight = position.height;
            position.height = 0;

            if (!((bool)skipDecoratorDrawers.GetValue(thisHandler)) && (List<DecoratorDrawer>)m_DecoratorDrawers.GetValue(thisHandler) != null && !((bool)isCurrentlyNested.GetValue(thisHandler)))
            {
                foreach (DecoratorDrawer decorator in (List<DecoratorDrawer>)m_DecoratorDrawers.GetValue(thisHandler))
                {
                    position.height = decorator.GetHeight();

                    oldLabelWidth = EditorGUIUtility.labelWidth;
                    oldFieldWidth = EditorGUIUtility.fieldWidth;
                    decorator.OnGUI(position);
                    EditorGUIUtility.labelWidth = oldLabelWidth;
                    EditorGUIUtility.fieldWidth = oldFieldWidth;

                    position.y += position.height;
                    propHeight -= position.height;
                }

                position.height = propHeight;
                if (propertyDrawer.GetValue(thisHandler) != null)
                {
                    // Remember widths
                    oldLabelWidth = EditorGUIUtility.labelWidth;
                    oldFieldWidth = EditorGUIUtility.fieldWidth;
                    // Draw with custom drawer - retrieve it BEFORE increasing nesting.
                    var drawer = propertyDrawer.GetValue(thisHandler);

                    using (var nestingContext = (System.IDisposable)IncrementNestingContext.Invoke(thisHandler, null))
                    {
                        OnGUISafe.Invoke(drawer, new object[] { position, property.Copy(), label ?? (GUIContent)TempContent.Invoke(null, new object[] { (string)localizedDisplayName.GetValue(property), (string)tooltip.GetValue(thisHandler) }) });
                    }

                    // Restore widths
                    EditorGUIUtility.labelWidth = oldLabelWidth;
                    EditorGUIUtility.fieldWidth = oldFieldWidth;

                    return false;
                }
                else
                {
                    if (!includeChildren)
                        return (bool)DefaultPropertyField.Invoke(null, new object[] { position, property, label });

                    if ((bool)UseReorderabelListControl.Invoke(thisHandler, new object[] { property }))
                    {
                        object reorderableList;
                        string key = (string)GetPropertyIdentifier.Invoke(null, new object[] { property });

                        Dictionary<string, object> tempDict = (Dictionary<string, object>)s_reorderableLists.GetValue(null);
                        if (!tempDict.TryGetValue(key, out reorderableList))
                        {
                            reorderableList = System.Activator.CreateInstance(ReorderableListWrapper, new object[] { property, label, true });
                            tempDict[key] = reorderableList;
                        }

                        // Calculate visibility rect specifically for reorderable list as when applied for the whole serialized object,
                        // it causes collapsed out of sight array elements appear thus messing up scroll-bar experience
                        Vector2 screenPos = GUIUtility.GUIToScreenPoint(position.position);

                        Rect listVisibility;
                        var cur = current.GetValue(null);
                        if (cur != null)
                        {
                            Rect curScreenPosition = (Rect)screenPosition.GetValue(cur);
                            screenPos.y = Mathf.Clamp(screenPos.y, curScreenPosition.yMin, curScreenPosition.yMax);
                            listVisibility = new Rect(screenPos.x, screenPos.y, curScreenPosition.width, curScreenPosition.height);
                        }
                        else
                        {
                            screenPos.y = Mathf.Clamp(screenPos.y, 0, UnityEngine.Screen.height);
                            listVisibility = new Rect(screenPos.x, screenPos.y, UnityEngine.Screen.width, UnityEngine.Screen.height);
                        }

                        listVisibility = GUIUtility.ScreenToGUIRect(listVisibility);

                        // Copy helps with recursive list rendering
                        Property.SetValue(reorderableList, property.Copy());
                        Draw.Invoke(reorderableList, new object[] { label, position, listVisibility, (string)tooltip.GetValue(thisHandler), includeChildren });
                        return !includeChildren && property.isExpanded;
                    }

                    // Remember state
                }
            }

            // Remember state
            Vector2 oldIconSize = EditorGUIUtility.GetIconSize();
            bool wasEnabled = GUI.enabled;
            int origIndent = EditorGUI.indentLevel;

            int relIndent = origIndent - property.depth;

            SerializedProperty prop = property.Copy();

            position.height = (float)GetSinglePropertyHeight.Invoke(null, new object[] { prop, label });

            // First property with custom label
            EditorGUI.indentLevel = prop.depth + relIndent;
            bool childrenAreExpanded = (bool)DefaultPropertyField.Invoke(null, new object[] { position, prop, label }) && (bool)HasVisibleChildFields.Invoke(null, new object[] { prop, false }); //Expanded => true && true
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing; // EditorGUIUtility.standardVerticalSpacing => EditorGUI.kControlVerticalSpacing

            if (property.isArray)
                BeginIsInsideList.Invoke(null, new object[] { prop.depth });

            // Loop through all child properties
            if (childrenAreExpanded)
            {
                SerializedProperty endProperty = prop.GetEndProperty();

                while (prop.NextVisible(childrenAreExpanded) && !SerializedProperty.EqualContents(prop, endProperty))
                {
                    if ((bool)isInsideList.GetValue(null) && prop.depth <= (int)GetInsideListDepth.Invoke(null, null))
                        EndIsInsideList.Invoke(null, null);

                    if (prop.isArray)
                        BeginIsInsideList.Invoke(null, new object[] { prop.depth });

                    var handler = GetHandler.Invoke(null, new object[] { prop });
                    EditorGUI.indentLevel = prop.depth + relIndent;
                    position.height = (float)GetHeight.Invoke(handler, new object[] { prop, null, (bool)UseReorderabelListControl.Invoke(null, new object[] { prop }) && includeChildren });

                    if (position.Overlaps(visibleArea))
                    {
                        EditorGUI.BeginChangeCheck();
                        childrenAreExpanded = (bool)OnGUI.Invoke(handler, new object[] { position, prop, null, (bool)UseReorderabelListControl.Invoke(null, new object[] { prop }) }) && (bool)HasVisibleChildFields.Invoke(null, new object[] { prop, false });
                        // Changing child properties (like array size) may invalidate the iterator,
                        // so stop now, or we may get errors.
                        if (EditorGUI.EndChangeCheck())
                            break;
                    }

                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            // Restore state
            if ((bool)isInsideList.GetValue(null) && property.depth <= (int)GetInsideListDepth.Invoke(null, null))
                EndIsInsideList.Invoke(null, null);
            GUI.enabled = wasEnabled;
            EditorGUIUtility.SetIconSize(oldIconSize);
            EditorGUI.indentLevel = origIndent;

            return false;
        }
    }
}
