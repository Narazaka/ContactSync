using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSync))]
    public class ContactSyncEditor : UnityEditor.Editor
    {
        SerializedProperty AssignGroups;
        ReorderableList AssignGroupsList;

        void OnEnable()
        {
            AssignGroups = serializedObject.FindProperty(nameof(ContactSync.AssignGroups));
            AssignGroupsList = new ReorderableList(serializedObject, AssignGroups);
            AssignGroupsList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Assign Groups", EditorStyles.boldLabel);
            AssignGroupsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = AssignGroupsList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element);
            };
            AssignGroupsList.elementHeightCallback = (int index) =>
            {
                var element = AssignGroupsList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AssignGroupsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
