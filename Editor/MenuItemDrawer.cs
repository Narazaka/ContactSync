using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomPropertyDrawer(typeof(MenuItem))]
    class MenuItemDrawer : PropertyDrawer
    {
        public static void PropertyField(SerializedProperty property, GUIContent label, string placeholder = null)
        {

            var drawer = new MenuItemDrawer();
            if (placeholder != null) drawer.Placeholder = placeholder;
            drawer.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Height(drawer.GetPropertyHeight(property, label))), property, label);
        }

        public string Placeholder;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var contactSyncSender = property.serializedObject.targetObject as ContactSyncSender;

            position = EditorGUI.IndentedRect(position);
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            GUI.Box(position, new GUIContent(), "flow node 0");

            position.yMin += EditorGUIUtility.standardVerticalSpacing;
            position.yMax -= EditorGUIUtility.standardVerticalSpacing;
            position.xMin += EditorGUIUtility.standardVerticalSpacing;
            position.xMax -= EditorGUIUtility.standardVerticalSpacing;

            var name = property.FindPropertyRelative(nameof(MenuItem.Name));
            var icon = property.FindPropertyRelative(nameof(MenuItem.Icon));

            var iconRect = position;
            iconRect.width = icon.objectReferenceValue == null ? 0 : position.height;
            iconRect.x = position.xMax - iconRect.width;
            var lineRect = position;
            lineRect.height = EditorGUIUtility.singleLineHeight;
            lineRect.width -= iconRect.width == 0 ? 0 : iconRect.width + 2;

            EditorGUI.LabelField(lineRect, label, EditorStyles.boldLabel);
            lineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUIHelper.PropertyField(lineRect, name, T.Name.GUIContent, Placeholder);
            lineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(lineRect, icon, T.Icon.GUIContent);

            if (iconRect.width != 0)
            {
                GUI.Box(iconRect, new GUIContent(), "flow node 1");
                GUI.DrawTexture(iconRect, icon.objectReferenceValue as Texture2D);
            }

            EditorGUI.indentLevel = indentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => Height;
        float Height = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 4;

        static class T
        {

            public static istring Name = new("Name", "名前");
            public static istring Icon = new("Icon", "アイコン");
        }
    }
}
