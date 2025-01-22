using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Narazaka.VRChat.ContactSync.Editor
{
    public abstract class ContactSyncCommunicatorEditor : ContactSyncPartsEditor
    {
        protected SerializedProperty TagName;
        protected Tag[] Tags;
        protected Tag Tag => Tags == null ? null : Tags.FirstOrDefault(t => t.Name == TagName.stringValue);

        TagDrawerHandler _tagDrawerHandler;

        protected virtual void OnEnable()
        {
            TagName = serializedObject.FindProperty(nameof(ContactSyncCommunicator.TagName));
        }

        protected override void UpdateVars()
        {
            base.UpdateVars();
            Tags = ContactSyncAssign != null && ContactSyncAssign.ContactSyncTagGroup != null && ContactSyncAssign.ContactSyncTagGroup.Tags != null ? ContactSyncAssign.ContactSyncTagGroup.Tags : null;
        }

        protected void DrawTagName()
        {
            if (Tags == null)
            {
                EditorGUILayout.PropertyField(TagName, T.TagName.GUIContent);
            }
            else
            {
                var tagNames = Tags.Select(t => t.Name).ToArray();
                var tag = Tag;
                var index = tag == null ? -1 : System.Array.IndexOf(Tags, tag);
                EditorGUI.BeginChangeCheck();
                var newIndex = EditorGUILayout.Popup(T.TagName.GUIContent, index, tagNames);
                if (EditorGUI.EndChangeCheck())
                {
                    TagName.stringValue = tagNames[newIndex];
                    _tagDrawerHandler = null;
                }
                if (DrawDetail)
                {
                    if (_tagDrawerHandler == null && newIndex != -1)
                    {
                        _tagDrawerHandler = new TagDrawerHandler(ContactSyncAssign.ContactSyncTagGroup, newIndex);
                    }
                    if (_tagDrawerHandler != null)
                    {
                        EditorGUI.indentLevel++;
                        _tagDrawerHandler.OnGUI();
                        EditorGUI.indentLevel--;
                        EditorGUILayoutHelper.DrawHorizontalLine();

                        TagName.stringValue = _tagDrawerHandler.TagName;
                    }
                }
            }
        }

        class TagDrawerHandler
        {
            SerializedObject SerializedObject;
            SerializedProperty TagProperty;
            SerializedProperty TagNameProperty;
            TagDrawer TagDrawer;

            public TagDrawerHandler(ContactSyncTagGroup contactSyncTagGroup, int index)
            {
                SerializedObject = new SerializedObject(contactSyncTagGroup);
                TagProperty = SerializedObject.FindProperty(nameof(ContactSyncTagGroup.Tags)).GetArrayElementAtIndex(index);
                TagNameProperty = TagProperty.FindPropertyRelative(nameof(Tag.Name));
                TagDrawer = new TagDrawer();
            }

            public void OnGUI()
            {
                SerializedObject.UpdateIfRequiredOrScript();
                TagDrawer.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Height(TagDrawer.GetPropertyHeight(TagProperty, T.TagName.GUIContent))), TagProperty, T.TagName.GUIContent);
                SerializedObject.ApplyModifiedProperties();
            }

            public string TagName => TagNameProperty.stringValue;
        }


        static class T
        {
            public static istring TagName => new("Tag", "タグ");
        }
    }
}
