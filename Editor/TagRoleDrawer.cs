using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomPropertyDrawer(typeof(TagRole))]
    class TagRoleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //position = EditorGUI.IndentedRect(position);
            //var indentLevel = EditorGUI.indentLevel;
            //EditorGUI.indentLevel = 0;

            var a = property.serializedObject.FindProperty(nameof(ContactSyncTagGroup.AName)).stringValue;
            var b = property.serializedObject.FindProperty(nameof(ContactSyncTagGroup.BName)).stringValue;
            if (string.IsNullOrEmpty(a)) a = "A";
            if (string.IsNullOrEmpty(b)) b = "B";
            property.enumValueIndex = EditorGUI.Popup(position, label, property.enumValueIndex, new GUIContent[] { new(a), new(b) });

            //EditorGUI.indentLevel = indentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}
