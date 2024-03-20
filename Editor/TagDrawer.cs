using UnityEngine;
using UnityEditor;

namespace net.narazaka.vrchat.contact_sync.editor
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
            var role = property.FindPropertyRelative(nameof(Tag.Role));
            var markExist = property.FindPropertyRelative(nameof(Tag.MarkExist));
            var continuous = property.FindPropertyRelative(nameof(Tag.Continuous));
            var separated = property.FindPropertyRelative(nameof(Tag.Separated));

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.width -= 77;
            EditorGUIUtility.labelWidth = 40;
            EditorGUI.PropertyField(rect, name, label.text == name.stringValue ? new GUIContent("Tag") : label);
            EditorGUIUtility.labelWidth = 0;

            rect.x += rect.width + 2;
            rect.width = 75;
            EditorGUI.PropertyField(rect, receiverType, GUIContent.none);

            rect = position;
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.width = 115;
            EditorGUIUtility.labelWidth = 25;
            EditorGUI.PropertyField(rect, role);
            EditorGUIUtility.labelWidth = 0;

            rect.x += rect.width + 2;
            rect.width = position.width - 117;
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
                EditorGUI.PropertyField(rect, markExist);
                EditorGUIUtility.labelWidth = 0;

                if (receiverType.enumValueIndex >= 4) // OnOff-
                {
                    rect.x += rect.width + 2;
                    rect.width = 82;
                    EditorGUIUtility.labelWidth = 65;
                    EditorGUI.PropertyField(rect, continuous);
                    EditorGUIUtility.labelWidth = 0;
                }
                if (receiverType.enumValueIndex == 4) // OnOff
                {
                    rect.x += rect.width + 2;
                    rect.width = 77;
                    EditorGUIUtility.labelWidth = 60;
                    EditorGUI.PropertyField(rect, separated);
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
