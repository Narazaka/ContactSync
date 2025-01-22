using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomPropertyDrawer(typeof(Tag))]
    class TagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var name = property.FindPropertyRelative(nameof(Tag.Name));
            var receiverType = property.FindPropertyRelative(nameof(Tag.ReceiverType));
            var minVelocity = property.FindPropertyRelative(nameof(Tag.MinVelocity));
            var sender = property.FindPropertyRelative(nameof(Tag.Sender));
            var memo = property.FindPropertyRelative(nameof(Tag.Memo));

            if (receiverType.enumValueIndex < ContactSyncReceiverTypeUtil.MinEnum) receiverType.enumValueIndex = (int)ContactSyncReceiverTypeUtil.DefaultEnum;

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.width -= 77;
            if (receiverType.enumValueIndex == 1) // OnEnter
            {
                rect.width -= 112;
            }
            EditorGUIUtility.labelWidth = 25;
            EditorGUI.PropertyField(rect, name, label.text == name.stringValue ? new GUIContent(T.Tag) : label);
            EditorGUIUtility.labelWidth = 0;

            if (receiverType.enumValueIndex == 1) // OnEnter
            {
                rect.x += rect.width + 2;
                rect.width = 110;
                EditorGUIUtility.labelWidth = 71;
                EditorGUI.PropertyField(rect, minVelocity);
                EditorGUIUtility.labelWidth = 0;
            }

            rect.x += rect.width + 2;
            rect.width = 75;
            EditorGUI.PropertyField(rect, receiverType, GUIContent.none);

            rect = position;
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.width = 140;
            EditorGUIUtility.labelWidth = 42;
            EditorGUI.PropertyField(rect, sender, T.Sender.GUIContent);
            EditorGUIUtility.labelWidth = 0;

            rect.x += rect.width + 2;
            rect.width = position.width - 142;
            EditorGUIUtility.labelWidth = 37;
            EditorGUI.PropertyField(rect, memo, T.Memo.GUIContent);
            EditorGUIUtility.labelWidth = 0;

            rect = position;
            rect.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.indentLevel = indentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lines = 2;
            return EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1);
        }

        static class T
        {
            public static istring Tag = new("Tag", "タグ");
            public static istring Sender = new("Sender", "送信者");
            public static istring Memo = new("Memo", "メモ");
        }
    }
}
