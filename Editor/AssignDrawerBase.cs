using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    class AssignDrawerBase : PropertyDrawer
    {
        protected class Props
        {
            public SerializedProperty name;
            public SerializedProperty enabled;
            public SerializedProperty enableMenu;
            public SerializedProperty parameterName;
            public SerializedProperty localOnly;
            public SerializedProperty menuName;
            public SerializedProperty icon;

            public bool effective;

            public Props(SerializedProperty property)
            {
                name = property.FindPropertyRelative(nameof(Assign.Name));
                enabled = property.FindPropertyRelative(nameof(Assign.Enabled));
                enableMenu = property.FindPropertyRelative(nameof(Assign.EnableMenu));
                parameterName = property.FindPropertyRelative(nameof(Assign.ParameterName));
                localOnly = property.FindPropertyRelative(nameof(Assign.LocalOnly));
                menuName = property.FindPropertyRelative(nameof(Assign.MenuName));
                icon = property.FindPropertyRelative(nameof(Assign.Icon));
                effective = enabled.boolValue || enableMenu.boolValue;
            }
        }

        protected void DrawNameLine(Rect position, Props props, GUIContent typeLabel, Color typeColor)
        {
            var rect = position;
            EditorGUIUtility.labelWidth = 35;
            using (new EditorGUI.DisabledGroupScope(!props.effective))
            {
                rect.width = 35;
                var normalColor = EditorStyles.label.normal.textColor;
                EditorStyles.label.normal.textColor = typeColor;
                EditorGUI.LabelField(rect, typeLabel);
                EditorStyles.label.normal.textColor = normalColor;
                rect.x += rect.width + 2;
                rect.width = position.width - 164 - 37;
                EditorGUI.LabelField(rect, props.name.stringValue, EditorStyles.boldLabel);
            }
            rect.x += rect.width + 2;
            rect.width = 70;
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(rect, props.enabled);
            rect.x += rect.width + 2;
            rect.width = 90;
            EditorGUIUtility.labelWidth = 75;
            EditorGUI.PropertyField(rect, props.enableMenu);

            EditorGUIUtility.labelWidth = 0;
        }

        protected void DrawParameterLine(Rect position, Props props)
        {
            var rect = position;
            rect.width = position.width - 82;
            EditorGUIUtility.labelWidth = 95;
            EditorGUI.PropertyField(rect, props.parameterName);
            rect.x += rect.width + 2;
            rect.width = 80;
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.PropertyField(rect, props.localOnly);

            EditorGUIUtility.labelWidth = 0;
        }

        protected void DrawMenuLine(Rect position, Props props)
        {
            var rect = position;
            rect.width = position.width / 2 - 1;
            EditorGUIUtility.labelWidth = 70;
            EditorGUI.PropertyField(rect, props.menuName);
            if (string.IsNullOrEmpty(props.menuName.stringValue))
            {
                var rect2 = rect;
                rect2.xMin += EditorGUIUtility.labelWidth + 2;
                var style = new GUIStyle(EditorStyles.miniLabel);
                style.normal.textColor = Color.gray;
                EditorGUI.LabelField(rect2, props.name.stringValue, style);
            }
            rect.x += rect.width + 2;
            EditorGUIUtility.labelWidth = 30;
            EditorGUI.PropertyField(rect, props.icon);

            EditorGUIUtility.labelWidth = 0;
        }
    }
}
