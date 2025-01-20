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
            var memo = property.FindPropertyRelative(nameof(Tag.Memo));
            var sendBy = property.FindPropertyRelative(nameof(Tag.SendBy));
            var notifyExists = property.FindPropertyRelative(nameof(Tag.NotifyExists));
            var locked = property.FindPropertyRelative(nameof(Tag.Locked));
            var allowUnconstrained = property.FindPropertyRelative(nameof(Tag.AllowUnconstrained));

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.width -= 77;
            EditorGUIUtility.labelWidth = 65;
            EditorGUI.PropertyField(rect, name, label.text == name.stringValue ? new GUIContent("Tag") : label);
            EditorGUIUtility.labelWidth = 0;

            rect.x += rect.width + 2;
            rect.width = 75;
            EditorGUI.PropertyField(rect, receiverType, GUIContent.none);

            rect = position;
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.width = 140;
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(rect, sendBy);
            EditorGUIUtility.labelWidth = 0;

            rect.x += rect.width + 2;
            rect.width = position.width - 142;
            EditorGUIUtility.labelWidth = 35;
            EditorGUI.PropertyField(rect, memo);
            EditorGUIUtility.labelWidth = 0;

            rect = position;
            rect.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (receiverType.enumValueIndex == 1) // OnEnter
            {
                rect.width = 110;
                EditorGUIUtility.labelWidth = 71;
                EditorGUI.PropertyField(rect, minVelocity);
                EditorGUIUtility.labelWidth = 0;
            }
            else if (receiverType.enumValueIndex >= 3) // Custom Types
            {
                rect.width = 77;
                EditorGUIUtility.labelWidth = 60;
                EditorGUI.PropertyField(rect, notifyExists);
                EditorGUIUtility.labelWidth = 0;

                if (receiverType.enumValueIndex >= 4) // OnOff-
                {
                    rect.x += rect.width + 2;
                    rect.width = 82;
                    EditorGUIUtility.labelWidth = 65;
                    EditorGUI.PropertyField(rect, locked);
                    EditorGUIUtility.labelWidth = 0;
                }
                if (receiverType.enumValueIndex == 4) // OnOff
                {
                    rect.x += rect.width + 2;
                    rect.width = 77;
                    EditorGUIUtility.labelWidth = 60;
                    EditorGUI.PropertyField(rect, allowUnconstrained);
                    EditorGUIUtility.labelWidth = 0;
                }
            }

            EditorGUI.indentLevel = indentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var receiverTypeIndex = property.FindPropertyRelative(nameof(Tag.ReceiverType)).enumValueIndex;
            var lines = receiverTypeIndex == 1 || receiverTypeIndex >= 3 ? 3 : 2;
            return EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1);
        }
    }
}
