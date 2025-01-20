using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    class AssignReceiverDrawer : AssignDrawerBase
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var props = new Props(property);

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            DrawNameLine(rect, props, new GUIContent("Recv"), Color.cyan);

            if (props.effective)
            {
                rect = position;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                DrawParameterLine(rect, props);

                rect = position;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
                DrawMenuLine(rect, props);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enabled = property.FindPropertyRelative(nameof(Assign.Enabled));
            var enableMenu = property.FindPropertyRelative(nameof(Assign.EnableMenu));
            var lines = enabled.boolValue || enableMenu.boolValue ? 3 : 1;
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lines;
        }
    }
}
