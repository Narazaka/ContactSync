using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    static class EditorGUIHelper
    {
        public static void PropertyField(Rect rect, SerializedProperty property, string placeholder)
        {
            EditorGUI.PropertyField(rect, property);
            DrawPlaceholder(rect, property, placeholder);
        }

        public static void PropertyField(Rect rect, SerializedProperty property, GUIContent label, string placeholder)
        {
            EditorGUI.PropertyField(rect, property, label);
            DrawPlaceholder(rect, property, placeholder);
        }

        static void DrawPlaceholder(Rect rect, SerializedProperty property, string placeholder)
        {
            if (!string.IsNullOrEmpty(placeholder) && string.IsNullOrEmpty(property.stringValue))
            {
                rect.width -= EditorGUIUtility.labelWidth + 6;
                rect.x += EditorGUIUtility.labelWidth + 3;
                EditorGUI.LabelField(rect, placeholder, PlaceholderStyle);
            }
        }

        static GUIStyle PlaceholderStyle => _placeholderStyle ??= new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic, normal = { textColor = Color.gray } };
        static GUIStyle _placeholderStyle;
    }
}
