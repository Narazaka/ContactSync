using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace net.narazaka.vrchat.contact_sync.editor
{
    [CustomPropertyDrawer(typeof(AssignGroup))]
    class AssignGroupDrawer : PropertyDrawer
    {
        Dictionary<ContactSyncTagGroup, ContactSyncTagGroupEditor> ContactSyncTagGroupEditorCache = new Dictionary<ContactSyncTagGroup, ContactSyncTagGroupEditor>();
        Dictionary<ContactSyncTagGroup, Dictionary<string, TagRole>> TagTypesCache = new Dictionary<ContactSyncTagGroup, Dictionary<string, TagRole>>();
        Dictionary<(ContactSyncTagGroup, string), ReorderableList> CommanderAssignsListCache = new Dictionary<(ContactSyncTagGroup, string), ReorderableList>();
        Dictionary<(ContactSyncTagGroup, string), ReorderableList> FollowerAssignsListCache = new Dictionary<(ContactSyncTagGroup, string), ReorderableList>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var contactSyncTagGroup = property.FindPropertyRelative(nameof(AssignGroup.ContactSyncTagGroup));
            var isCommander = property.FindPropertyRelative(nameof(AssignGroup.IsCommander));
            var isFollower = property.FindPropertyRelative(nameof(AssignGroup.IsFollower));
            var commanderAssigns = property.FindPropertyRelative(nameof(AssignGroup.CommanderAssigns));
            var followerAssigns = property.FindPropertyRelative(nameof(AssignGroup.FollowerAssigns));

            var contactSyncTagGroupObject = contactSyncTagGroup.objectReferenceValue as ContactSyncTagGroup;

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = position.width - 42;
            EditorGUI.PropertyField(rect, contactSyncTagGroup);

            rect.x += rect.width + 2;
            rect.width = 40;
            if (contactSyncTagGroupObject == null)
            {
                if (GUI.Button(rect, "New"))
                {
                    System.IO.Directory.CreateDirectory("Assets/ContactSyncTagGroups");
                    var defaultPath = AssetDatabase.GenerateUniqueAssetPath("Assets/ContactSyncTagGroups/ContactSyncTagGroup.asset");
                    var path = EditorUtility.SaveFilePanelInProject("Create ContactSyncTagGroup", System.IO.Path.GetFileNameWithoutExtension(defaultPath), "asset", "", "Assets/ContactSyncTagGroups");
                    if (string.IsNullOrEmpty(path)) return;
                    var asset = ScriptableObject.CreateInstance<ContactSyncTagGroup>();
                    asset.Prefix = GUID.Generate().ToString();
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    contactSyncTagGroup.objectReferenceValue = asset;
                }
                return;
            }
            else
            {
                if (GUI.Button(rect, "Edit"))
                {
                    contactSyncTagGroup.isExpanded = !contactSyncTagGroup.isExpanded;
                }
            }

            var editorHeight = 0f;
            if (contactSyncTagGroup.isExpanded)
            {
                if (!ContactSyncTagGroupEditorCache.TryGetValue(contactSyncTagGroupObject, out var contactSyncTagGroupEditor))
                {
                    ContactSyncTagGroupEditorCache[contactSyncTagGroupObject] = contactSyncTagGroupEditor = Editor.CreateEditor(contactSyncTagGroupObject) as ContactSyncTagGroupEditor;
                }
                editorHeight = contactSyncTagGroupEditor.GetHeight();
                rect = position;
                rect.height = editorHeight;
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                contactSyncTagGroupEditor.OnEditorGUI(rect);
            }

            var tagTypes = TagTypesDictionary(contactSyncTagGroupObject, true);

            rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + editorHeight;
            rect.width = position.width / 2;
            EditorGUIUtility.labelWidth = 85;
            var normalColor = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = Color.red;
            isCommander.boolValue = EditorGUI.ToggleLeft(rect, "Commander", isCommander.boolValue);
            rect.x += rect.width;
            EditorStyles.label.normal.textColor = Color.cyan;
            isFollower.boolValue = EditorGUI.ToggleLeft(rect, "Follower", isFollower.boolValue);
            EditorStyles.label.normal.textColor = normalColor;
            EditorGUIUtility.labelWidth = 0;
            rect = position;
            rect.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2 + editorHeight;
            if (isCommander.boolValue)
            {
                RecoverAssigns(tagTypes, commanderAssigns);
                var commanderAssignsList = AssignsList(contactSyncTagGroupObject, commanderAssigns, CommanderAssignsListCache, true);
                rect.height = commanderAssignsList.GetHeight();
                commanderAssignsList.DoList(rect);
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }
            if (isFollower.boolValue)
            {
                RecoverAssigns(tagTypes, followerAssigns);
                var followerAssignsList = AssignsList(contactSyncTagGroupObject, followerAssigns, FollowerAssignsListCache, false);
                rect.height = followerAssignsList.GetHeight();
                followerAssignsList.DoList(rect);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var contactSyncTagGroup = property.FindPropertyRelative(nameof(AssignGroup.ContactSyncTagGroup));
            var isCommander = property.FindPropertyRelative(nameof(AssignGroup.IsCommander));
            var isFollower = property.FindPropertyRelative(nameof(AssignGroup.IsFollower));

            var contactSyncTagGroupObject = contactSyncTagGroup.objectReferenceValue as ContactSyncTagGroup;

            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (contactSyncTagGroupObject != null)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (contactSyncTagGroup.isExpanded)
                {
                    if (!ContactSyncTagGroupEditorCache.TryGetValue(contactSyncTagGroupObject, out var contactSyncTagGroupEditor))
                    {
                        ContactSyncTagGroupEditorCache[contactSyncTagGroupObject] = contactSyncTagGroupEditor = Editor.CreateEditor(contactSyncTagGroupObject) as ContactSyncTagGroupEditor;
                    }
                    height += contactSyncTagGroupEditor.GetHeight();
                }
                if (isCommander.boolValue)
                {
                    var commanderAssigns = property.FindPropertyRelative(nameof(AssignGroup.CommanderAssigns));
                    height += AssignsList(contactSyncTagGroupObject, commanderAssigns, CommanderAssignsListCache, true).GetHeight() + EditorGUIUtility.standardVerticalSpacing;
                }
                if (isFollower.boolValue)
                {
                    var followerAssigns = property.FindPropertyRelative(nameof(AssignGroup.FollowerAssigns));
                    height += AssignsList(contactSyncTagGroupObject, followerAssigns, FollowerAssignsListCache, false).GetHeight() + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }

        ReorderableList AssignsList(ContactSyncTagGroup contactSyncTagGroup, SerializedProperty property, Dictionary<(ContactSyncTagGroup, string), ReorderableList> assignsListCache, bool isCommander)
        {
            if (assignsListCache.TryGetValue((contactSyncTagGroup, property.propertyPath), out var assignsList)) return assignsList;
            assignsList = new ReorderableList(property.serializedObject, property, true, true, false, false);
            var style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = isCommander ? Color.red : Color.cyan;
            assignsList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, property.displayName, style);
            assignsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = assignsList.serializedProperty.GetArrayElementAtIndex(index);
                var name = element.FindPropertyRelative(nameof(Assign.Name)).stringValue;
                var tagTypes = TagTypesDictionary(contactSyncTagGroup);
                if (!tagTypes.TryGetValue(name, out var tagType))
                {
                    EditorGUI.LabelField(rect, $"Element {index} [{name}] is not found in {contactSyncTagGroup.name}");
                    return;
                }
                PropertyDrawer drawer = isCommander == (tagType == TagRole.Commander) ? (PropertyDrawer)new AssignSenderDrawer() : new AssignReceiverDrawer();
                drawer.OnGUI(rect, element, new GUIContent($"Element {index}"));
            };
            assignsList.elementHeightCallback = (index) =>
            {
                var element = assignsList.serializedProperty.GetArrayElementAtIndex(index);
                var name = element.FindPropertyRelative(nameof(Assign.Name)).stringValue;
                var tagTypes = TagTypesDictionary(contactSyncTagGroup);
                if (!tagTypes.TryGetValue(name, out var tagType))
                {
                    return EditorGUIUtility.singleLineHeight;
                }
                PropertyDrawer drawer = isCommander == (tagType == TagRole.Commander) ? (PropertyDrawer)new AssignSenderDrawer() : new AssignReceiverDrawer();
                return drawer.GetPropertyHeight(element, new GUIContent($"Element {index}"));
            };
            assignsListCache[(contactSyncTagGroup, property.propertyPath)] = assignsList;
            return assignsList;
        }

        void RecoverAssigns(Dictionary<string, TagRole> tagTypes, SerializedProperty assigns)
        {
            if (tagTypes.Count == 0)
            {
                assigns.ClearArray();
                return;
            }
            var names = new HashSet<string>(tagTypes.Keys);
            var extraIndexes = new List<int>();
            for (var i = 0; i < assigns.arraySize; i++)
            {
                var assign = assigns.GetArrayElementAtIndex(i);
                var name = assign.FindPropertyRelative(nameof(Assign.Name)).stringValue;
                if (!names.Remove(name))
                {
                    extraIndexes.Add(i);
                }
            }
            extraIndexes.Reverse();
            foreach (var extraIndex in extraIndexes)
            {
                assigns.DeleteArrayElementAtIndex(extraIndex);
            }
            foreach (var name in names)
            {
                var index = assigns.arraySize;
                assigns.InsertArrayElementAtIndex(index);
                var assign = assigns.GetArrayElementAtIndex(index);
                assign.FindPropertyRelative(nameof(Assign.Name)).stringValue = name;
                assign.FindPropertyRelative(nameof(Assign.Enabled)).boolValue = true;
                assign.FindPropertyRelative(nameof(Assign.EnableMenu)).boolValue = false;
                assign.FindPropertyRelative(nameof(Assign.ParameterName)).stringValue = name;
                assign.FindPropertyRelative(nameof(Assign.LocalOnly)).boolValue = true;
            }
        }

        Dictionary<string, TagRole> TagTypesDictionary(ContactSyncTagGroup contactSyncTagGroup, bool forceUpdate = false)
        {
            if (contactSyncTagGroup?.Tags == null) return new Dictionary<string, TagRole>();
            if (TagTypesCache.TryGetValue(contactSyncTagGroup, out var tagTypes) && !forceUpdate) return tagTypes;
            tagTypes = contactSyncTagGroup.Tags.Distinct(new TagNameComparer()).ToDictionary(t => t.Name, t => t.SendBy);
            TagTypesCache[contactSyncTagGroup] = tagTypes;
            return tagTypes;
        }

        class TagNameComparer : IEqualityComparer<Tag>
        {
            public bool Equals(Tag x, Tag y) => x.Name == y.Name;

            public int GetHashCode(Tag obj) => obj.Name.GetHashCode();
        }
    }
}
