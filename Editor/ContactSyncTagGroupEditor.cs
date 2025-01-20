using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSyncTagGroup))]
    public class ContactSyncTagGroupEditor : UnityEditor.Editor
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
            TagsList.onAddCallback = (ReorderableList list) =>
            {
                Tags.arraySize++;
                if (Tags.arraySize == 1)
                {
                    var tag = Tags.GetArrayElementAtIndex(0);
                    tag.FindPropertyRelative(nameof(Tag.ReceiverType)).enumValueIndex = 4;
                    tag.FindPropertyRelative(nameof(Tag.MarkExist)).boolValue = true;
                    tag.FindPropertyRelative(nameof(Tag.Continuous)).boolValue = true;
                    tag.FindPropertyRelative(nameof(Tag.Separated)).boolValue = false;
                }
            };
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
