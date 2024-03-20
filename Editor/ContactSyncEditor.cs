using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Security.Cryptography;

namespace net.narazaka.vrchat.contact_sync.editor
{
    [CustomEditor(typeof(ContactSync))]
    public class ContactSyncEditor : Editor
    {
        SerializedProperty TagGroups;
        SerializedProperty AssignGroups;
        ReorderableList TagGroupsList;
        Dictionary<Object, Editor> ContactSyncTagGroupEditorCache;

        void OnEnable()
        {
            TagGroups = serializedObject.FindProperty(nameof(ContactSync.TagGroups));
            AssignGroups = serializedObject.FindProperty(nameof(ContactSync.AssignGroups));
            ContactSyncTagGroupEditorCache = new Dictionary<Object, Editor>();
            TagGroupsList = CreateTagGroupList(TagGroups);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TagGroupsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        ReorderableList CreateTagGroupList(SerializedProperty property, string label = null)
        {
            var sendLabelContent = new GUIContent("Send");
            var receiveLabelContent = new GUIContent("Receive");
            var parameterLabelContent = new GUIContent("Parameter");
            var list = new ReorderableList(serializedObject, property);
            list.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, label ?? property.displayName, EditorStyles.boldLabel);
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                if (element.objectReferenceValue == null)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                    if (element.objectReferenceValue != null)
                    {
                        // ensure unique
                        for (var i = 0; i < list.serializedProperty.arraySize; ++i)
                        {
                            if (i == index) continue;
                            var otherElement = list.serializedProperty.GetArrayElementAtIndex(i);
                            if (otherElement.objectReferenceValue == element.objectReferenceValue)
                            {
                                element.objectReferenceValue = null;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                    }
                }
                if (element.objectReferenceValue == null) return;
                if (!ContactSyncTagGroupEditorCache.TryGetValue(element.objectReferenceValue, out var editor))
                {
                    ContactSyncTagGroupEditorCache[element.objectReferenceValue] = editor = CreateEditor(element.objectReferenceValue);
                }
                var contactSyncTagGroupEditor = editor as ContactSyncTagGroupEditor;
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                rect.height = contactSyncTagGroupEditor.GetHeight();
                contactSyncTagGroupEditor.OnEditorGUI(rect);

                var contactSyncGroup = element.objectReferenceValue as ContactSyncTagGroup;
                var tagsSize = contactSyncGroup.Tags.Length;
                if (tagsSize == 0) return;

                // migration 可能な限り復元してマッチ
                var assignGroup = AssignGroups.arraySize > index ? AssignGroups.GetArrayElementAtIndex(index) : null;
                if (assignGroup == null || assignGroup.FindPropertyRelative(nameof(AssignGroup.ContactSyncTagGroup)).objectReferenceValue != element.objectReferenceValue)
                {
                    for (var i = 0; i < AssignGroups.arraySize; ++i)
                    {
                        var otherAssignGroup = AssignGroups.GetArrayElementAtIndex(i);
                        if (otherAssignGroup.FindPropertyRelative(nameof(AssignGroup.ContactSyncTagGroup)).objectReferenceValue == element.objectReferenceValue)
                        {
                            assignGroup = otherAssignGroup;
                            break;
                        }
                    }
                    if (assignGroup == null)
                    {
                        if (AssignGroups.arraySize > index)
                        {
                            if (AssignGroups.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(AssignGroup.ContactSyncTagGroup)).objectReferenceValue != null)
                            {
                                AssignGroups.InsertArrayElementAtIndex(index);
                            }
                        }
                        else
                        {
                            AssignGroups.arraySize = index + 1;
                        }
                        assignGroup = AssignGroups.GetArrayElementAtIndex(index);
                        assignGroup.FindPropertyRelative(nameof(AssignGroup.ContactSyncTagGroup)).objectReferenceValue = element.objectReferenceValue;
                    }
                }
                var assigns = assignGroup.FindPropertyRelative(nameof(AssignGroup.Assigns));
                if (assigns.arraySize < tagsSize)
                {
                    assigns.arraySize = tagsSize;
                }
                var replaces = new Dictionary<int, Assign>();
                for (int i = 0; i < tagsSize; i++)
                {
                    var assign = assigns.GetArrayElementAtIndex(i);
                    var tag = contactSyncGroup.Tags[i];
                    var name = assign.FindPropertyRelative(nameof(Assign.Name));
                    if (string.IsNullOrEmpty(name.stringValue))
                    {
                        name.stringValue = tag.Name;
                    }
                    else if (name.stringValue != tag.Name)
                    {
                        var replaced = false;
                        for (int j = 0; j < tagsSize; j++)
                        {
                            var otherAssign = assigns.GetArrayElementAtIndex(j);
                            if (otherAssign.FindPropertyRelative(nameof(Assign.Name)).stringValue == tag.Name)
                            {
                                replaces.Add(i, new Assign
                                {
                                    IsSend = otherAssign.FindPropertyRelative(nameof(Assign.IsSend)).boolValue,
                                    IsReceive = otherAssign.FindPropertyRelative(nameof(Assign.IsReceive)).boolValue,
                                    Name = tag.Name,
                                    ParameterName = otherAssign.FindPropertyRelative(nameof(Assign.ParameterName)).stringValue,
                                    LocalOnly = otherAssign.FindPropertyRelative(nameof(Assign.LocalOnly)).boolValue,
                                });
                                replaced = true;
                                break;
                            }
                        }
                        if (!replaced)
                        {
                            replaces.Add(i, new Assign
                            {
                                IsSend = assign.FindPropertyRelative(nameof(Assign.IsSend)).boolValue,
                                IsReceive = assign.FindPropertyRelative(nameof(Assign.IsReceive)).boolValue,
                                Name = tag.Name,
                                ParameterName = assign.FindPropertyRelative(nameof(Assign.ParameterName)).stringValue,
                                LocalOnly = assign.FindPropertyRelative(nameof(Assign.LocalOnly)).boolValue,
                            });
                        }
                    }
                }
                if (replaces.Count > 0)
                {
                    foreach (var replace in replaces)
                    {
                        var assign = assigns.GetArrayElementAtIndex(replace.Key);
                        assign.FindPropertyRelative(nameof(Assign.IsSend)).boolValue = replace.Value.IsSend;
                        assign.FindPropertyRelative(nameof(Assign.IsReceive)).boolValue = replace.Value.IsReceive;
                        assign.FindPropertyRelative(nameof(Assign.Name)).stringValue = replace.Value.Name;
                        assign.FindPropertyRelative(nameof(Assign.ParameterName)).stringValue = replace.Value.ParameterName;
                        assign.FindPropertyRelative(nameof(Assign.LocalOnly)).boolValue = replace.Value.LocalOnly;
                    }
                }
                assigns.arraySize = tagsSize;
                // end migration

                var x = rect.x;
                var width = rect.width;
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = width - 160;
                EditorGUI.LabelField(rect, "Behaviour", EditorStyles.boldLabel);
                rect.x += rect.width;
                rect.width = 55;
                if (GUI.Button(rect, "Sender"))
                {
                    for (int i = 0; i < tagsSize; i++)
                    {
                        var assign = assigns.GetArrayElementAtIndex(i);
                        assign.FindPropertyRelative(nameof(Assign.IsSend)).boolValue = true;
                        assign.FindPropertyRelative(nameof(Assign.IsReceive)).boolValue = false;
                    }
                }
                rect.x += rect.width;
                rect.width = 65;
                if (GUI.Button(rect, "Receiver"))
                {
                    for (int i = 0; i < tagsSize; i++)
                    {
                        var assign = assigns.GetArrayElementAtIndex(i);
                        assign.FindPropertyRelative(nameof(Assign.IsSend)).boolValue = false;
                        assign.FindPropertyRelative(nameof(Assign.IsReceive)).boolValue = true;
                    }
                }
                rect.x += rect.width;
                rect.width = 40;
                if (GUI.Button(rect, "Both"))
                {
                    for (int i = 0; i < tagsSize; i++)
                    {
                        var assign = assigns.GetArrayElementAtIndex(i);
                        assign.FindPropertyRelative(nameof(Assign.IsSend)).boolValue = true;
                        assign.FindPropertyRelative(nameof(Assign.IsReceive)).boolValue = true;
                    }
                }
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                for (int i = 0; i < tagsSize; i++)
                {
                    var assign = assigns.GetArrayElementAtIndex(i);
                    var tag = contactSyncGroup.Tags[i];
                    var isSend = assign.FindPropertyRelative(nameof(Assign.IsSend));
                    var isReceive = assign.FindPropertyRelative(nameof(Assign.IsReceive));
                    var name = assign.FindPropertyRelative(nameof(Assign.Name));
                    var parameterName = assign.FindPropertyRelative(nameof(Assign.ParameterName));
                    var localOnly = assign.FindPropertyRelative(nameof(Assign.LocalOnly));
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.x = x;
                    rect.width = width - 135;
                    EditorGUI.LabelField(rect, $"Tag: {tag.Name}");
                    rect.x += rect.width;
                    rect.width = 55;
                    EditorGUIUtility.labelWidth = 35;
                    EditorGUI.PropertyField(rect, isSend, sendLabelContent);
                    rect.x += rect.width + 10;
                    rect.width = 70;
                    EditorGUIUtility.labelWidth = 50;
                    EditorGUI.PropertyField(rect, isReceive, receiveLabelContent);
                    EditorGUIUtility.labelWidth = 0;
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    if (!isReceive.boolValue) continue;
                    rect.x = x + 20;
                    rect.width = width - 105;
                    EditorGUIUtility.labelWidth = 60;
                    EditorGUI.PropertyField(rect, parameterName, parameterLabelContent);
                    rect.x += rect.width;
                    rect.width = 85;
                    EditorGUI.LabelField(new Rect(rect.x - 30, rect.y, 30, rect.height), tag.ReceiverType == ContactSyncReceiverType.Proximity ? "0-1" : "0or1", EditorStyles.centeredGreyMiniLabel);
                    EditorGUIUtility.labelWidth = 65;
                    EditorGUI.PropertyField(rect, localOnly);
                    EditorGUIUtility.labelWidth = 0;
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                }
            };
            list.elementHeightCallback = (int index) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                if (element.objectReferenceValue == null) return EditorGUIUtility.singleLineHeight;
                if (!ContactSyncTagGroupEditorCache.TryGetValue(element.objectReferenceValue, out var editor))
                {
                    ContactSyncTagGroupEditorCache[element.objectReferenceValue] = editor = CreateEditor(element.objectReferenceValue);
                }
                var contactSyncTagGroupEditor = editor as ContactSyncTagGroupEditor;
                var assignGroup = AssignGroups.arraySize > index ? AssignGroups.GetArrayElementAtIndex(index) : null;
                var lineCount = 0;
                if (assignGroup != null && contactSyncTagGroupEditor.GetCount() > 0)
                {
                    var assigns = assignGroup.FindPropertyRelative(nameof(AssignGroup.Assigns));
                    lineCount = assigns.arraySize + 1;
                    for (var i = 0; i < assigns.arraySize; ++i)
                    {
                        var assign = assigns.GetArrayElementAtIndex(i);
                        if (assign.FindPropertyRelative(nameof(Assign.IsReceive)).boolValue)
                        {
                            ++lineCount;
                        }
                    }
                }
                return contactSyncTagGroupEditor.GetHeight() + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (1 + lineCount) + EditorGUIUtility.standardVerticalSpacing;
            };
            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList _list) =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create New"), false, () =>
                {
                    Directory.CreateDirectory("Assets/ContactSyncTagGroups");
                    var defaultPath = AssetDatabase.GenerateUniqueAssetPath("Assets/ContactSyncTagGroups/ContactSyncTagGroup.asset");
                    var path = EditorUtility.SaveFilePanelInProject("Create ContactSyncTagGroup", Path.GetFileNameWithoutExtension(defaultPath), "asset", "", "Assets/ContactSyncTagGroups");
                    if (string.IsNullOrEmpty(path)) return;
                    var asset = CreateInstance<ContactSyncTagGroup>();
                    asset.Prefix = GUID.Generate().ToString();
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.CreateAsset(asset, path);
                    var index = list.serializedProperty.arraySize;
                    list.serializedProperty.arraySize++;
                    list.index = index;
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    element.objectReferenceValue = asset;
                    list.serializedProperty.serializedObject.ApplyModifiedProperties();
                });
                var existAssetPaths = new HashSet<string>();
                for (var i = 0; i < list.serializedProperty.arraySize; ++i)
                {
                    var tagGroup = list.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (tagGroup != null) existAssetPaths.Add(AssetDatabase.GetAssetPath(tagGroup));
                }
                var guids = AssetDatabase.FindAssets("t:ContactSyncTagGroup");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (existAssetPaths.Contains(path)) continue;
                    menu.AddItem(new GUIContent($"Add [{Path.GetFileNameWithoutExtension(path)}]"), false, () =>
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<ContactSyncTagGroup>(path);
                        var index = list.serializedProperty.arraySize;
                        list.serializedProperty.arraySize++;
                        list.index = index;
                        var element = list.serializedProperty.GetArrayElementAtIndex(index);
                        element.objectReferenceValue = asset;
                        list.serializedProperty.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            };
            list.onRemoveCallback = (ReorderableList _list) =>
            {
                var index = list.index;
                list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = null;
                list.serializedProperty.DeleteArrayElementAtIndex(index);
            };
            return list;
        }
    }
}
