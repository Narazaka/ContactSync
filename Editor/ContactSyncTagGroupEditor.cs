using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace net.narazaka.vrchat.contact_sync.editor
{
    [CustomEditor(typeof(ContactSyncTagGroup))]
    public class ContactSyncTagGroupEditor : Editor
    {
        SerializedProperty Name;
        SerializedProperty Tags;
#if !UNITY_2020_3_OR_NEWER
        ReorderableList TagsList;
#endif

        void OnEnable()
        {
            var tagNameLabel = new GUIContent("Tag");
            Name = serializedObject.FindProperty(nameof(ContactSyncTagGroup.Prefix));
            Tags = serializedObject.FindProperty(nameof(ContactSyncTagGroup.Tags));
#if !UNITY_2020_3_OR_NEWER
            TagsList = new ReorderableList(serializedObject, Tags);
            TagsList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Tags");
            TagsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => EditorGUI.PropertyField(rect, Tags.GetArrayElementAtIndex(index));
            TagsList.elementHeightCallback = (int index) => EditorGUI.GetPropertyHeight(Tags.GetArrayElementAtIndex(index));
#endif
        }

        public override void OnInspectorGUI()
        {
            OnEditorGUI(EditorGUILayout.GetControlRect(false, GetHeight()));
        }

        public void OnEditorGUI(Rect rect)
        {
            serializedObject.Update();

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), Name);
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
#if UNITY_2020_3_OR_NEWER
            EditorGUI.PropertyField(rect, Tags);
#else
            TagsList.DoList(rect);
#endif

            serializedObject.ApplyModifiedProperties();
        }

        public float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
#if UNITY_2020_3_OR_NEWER
                EditorGUI.GetPropertyHeight(Tags);
#else
                TagsList.GetHeight();
#endif
        }

        public int GetCount()
        {
            return Tags.arraySize;
        }
    }
}
