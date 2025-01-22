using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    static class EditorGUILayoutHelper
    {
        public static void DrawHorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public static void PropertyField(SerializedProperty property, string placeholder)
        {
            var rect = EditorGUILayout.GetControlRect();
            EditorGUIHelper.PropertyField(rect, property, placeholder);
        }

        public static void PropertyField(SerializedProperty property, GUIContent label, string placeholder)
        {
            var rect = EditorGUILayout.GetControlRect();
            EditorGUIHelper.PropertyField(rect, property, label, placeholder);
        }
    }
}
