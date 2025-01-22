using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using nadena.dev.modular_avatar.core;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSyncAssign))]
    class ContactSyncAssignDrawer : UnityEditor.Editor
    {
        SerializedProperty ContactSyncTagGroup;
        SerializedProperty AllowSelfContact;
        ContactSyncTagGroupEditor ContactSyncTagGroupEditor;
        ContactSyncTagGroup ContactSyncTagGroupObject;
        Tag[] Tags => ContactSyncTagGroupObject == null ? new Tag[0] : ContactSyncTagGroupObject.Tags;
        string EffectiveAName => ContactSyncTagGroupObject == null ? "A" : ContactSyncTagGroupObject.EffectiveAName;
        string EffectiveBName => ContactSyncTagGroupObject == null ? "B" : ContactSyncTagGroupObject.EffectiveBName;
        ContactSyncParts[] ContactSyncParts;
        bool ShowContactSyncParts = true;
        Dictionary<ContactSyncParts, ContactSyncPartsEditor> ContactSyncPartsEditors = new Dictionary<ContactSyncParts, ContactSyncPartsEditor>();
        GUIStyle ContactSyncPartsHeaderLabelStyle;

        void OnEnable()
        {
            ContactSyncTagGroup = serializedObject.FindProperty(nameof(ContactSyncAssign.ContactSyncTagGroup));
            AllowSelfContact = serializedObject.FindProperty(nameof(ContactSyncAssign.AllowSelfContact));
            ContactSyncPartsHeaderLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            ContactSyncParts = new ContactSyncParts[0];
        }

        public override void OnInspectorGUI()
        {
            UpdateParts();

            serializedObject.UpdateIfRequiredOrScript();

            ContactSyncTagGroupObject = ContactSyncTagGroup.objectReferenceValue as ContactSyncTagGroup;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(ContactSyncTagGroup, T.ContactSyncTagGroup.GUIContent);

            if (ContactSyncTagGroupObject == null)
            {
                if (GUILayout.Button(T.New, GUILayout.Width(60)))
                {
                    System.IO.Directory.CreateDirectory("Assets/ContactSyncTagGroups");
                    var defaultPath = AssetDatabase.GenerateUniqueAssetPath("Assets/ContactSyncTagGroups/ContactSyncTagGroup.asset");
                    var path = EditorUtility.SaveFilePanelInProject("Create ContactSyncTagGroup", System.IO.Path.GetFileNameWithoutExtension(defaultPath), "asset", "", "Assets/ContactSyncTagGroups");
                    if (string.IsNullOrEmpty(path)) return;
                    var asset = ScriptableObject.CreateInstance<ContactSyncTagGroup>();
                    asset.Name = GUID.Generate().ToString();
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    ContactSyncTagGroup.objectReferenceValue = asset;
                }
            }
            else
            {
                if (GUILayout.Button(T.Edit, GUILayout.Width(60)))
                {
                    ContactSyncTagGroup.isExpanded = !ContactSyncTagGroup.isExpanded;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (ContactSyncTagGroupObject != null && ContactSyncTagGroup.isExpanded)
            {
                if (ContactSyncTagGroupEditor == null || ContactSyncTagGroupEditor.target != ContactSyncTagGroupObject)
                {
                    ContactSyncTagGroupEditor = CreateEditor(ContactSyncTagGroupObject) as ContactSyncTagGroupEditor;
                    ContactSyncPartsEditors.Clear();
                }
                ContactSyncTagGroupEditor.OnInspectorGUI();
            }

            EditorGUILayout.PropertyField(AllowSelfContact, T.AllowSelfContact.GUIContent);

            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginDisabledGroup((target as Component).GetComponent<ModularAvatarMenuInstaller>() != null);
            if (GUILayout.Button(T.AddMAMenuInstaller))
            {
                var c = (target as Component).gameObject.AddComponent<ModularAvatarMenuInstaller>();
                UnityEditorInternal.ComponentUtility.MoveComponentUp(c);
                Undo.RegisterCreatedObjectUndo(c, "Create MA Menu Installer");
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup((target as Component).GetComponent<ModularAvatarMenuGroup>() != null);
            if (GUILayout.Button(T.AddMAMenuGroup))
            {
                var c = (target as Component).gameObject.AddComponent<ModularAvatarMenuGroup>();
                UnityEditorInternal.ComponentUtility.MoveComponentUp(c);
                Undo.RegisterCreatedObjectUndo(c, "Create MA Menu Group");
            }
            EditorGUI.EndDisabledGroup();

            var hasAllToggle = ContactSyncParts.Any(parts => parts is ContactSyncAllToggle);
            EditorGUI.BeginDisabledGroup(hasAllToggle);
            if (GUILayout.Button(T.AddAllToggle))
            {
                var go = new GameObject("All Toggle");
                go.transform.SetParent((target as ContactSyncAssign).transform, false);
                go.transform.SetAsFirstSibling();
                go.AddComponent<ContactSyncAllToggle>();
                Undo.RegisterCreatedObjectUndo(go, "Create ContactSyncAllToggle");
            }
            EditorGUI.EndDisabledGroup();

            var hasMatchKey = ContactSyncParts.Any(parts => parts is ContactSyncMatchKey);
            EditorGUI.BeginDisabledGroup(hasMatchKey);
            if (GUILayout.Button(T.AddMatchKey))
            {
                var go = new GameObject("Match Key");
                go.transform.SetParent((target as ContactSyncAssign).transform, false);
                if (hasAllToggle)
                {
                    var allToggle = ContactSyncParts.First(parts => parts is ContactSyncAllToggle);
                    go.transform.SetSiblingIndex(allToggle.transform.GetSiblingIndex() + 1);
                }
                else
                {
                    go.transform.SetAsFirstSibling();
                }
                go.AddComponent<ContactSyncMatchKey>();
                Undo.RegisterCreatedObjectUndo(go, "Create ContactSyncMatchKey");
            }
            EditorGUI.EndDisabledGroup();

            var tagAssignInfo = new TagAssignInfo(Tags, ContactSyncParts);

            EditorGUI.BeginDisabledGroup(tagAssignInfo.ALackCount == 0);
            if (GUILayout.Button(T.CreateAssigns(EffectiveAName, tagAssignInfo.ALackCount)))
            {
                CreateLackAssigns(TagRole.A);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(tagAssignInfo.BLackCount == 0);
            if (GUILayout.Button(T.CreateAssigns(EffectiveBName, tagAssignInfo.BLackCount)))
            {
                CreateLackAssigns(TagRole.B);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(tagAssignInfo.UnassignedCount == 0);
            if (GUILayout.Button(T.RemoveUnassigned))
            {
                RemoveUnassigned();
            }
            EditorGUI.EndDisabledGroup();

            if (ContactSyncParts.Length > 0)
            {
                ShowContactSyncParts = EditorGUILayout.Foldout(ShowContactSyncParts, T.ShowContactSyncParts);
                if (ShowContactSyncParts)
                {
                    var senderNameWidth = CalcSenderNameWidth();
                    EditorGUI.indentLevel++;
                    foreach (var part in ContactSyncParts)
                    {
                        if (part != null && DrawContactSyncPartsHeader(part, senderNameWidth))
                        {
                            EditorGUI.indentLevel++;
                            if (!ContactSyncPartsEditors.TryGetValue(part, out var editor) || editor == null)
                            {
                                editor = CreateEditor(part) as ContactSyncPartsEditor;
                                editor.DrawDetail = false;
                                ContactSyncPartsEditors[part] = editor;
                            }
                            editor.OnInspectorGUI();
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        float CalcSenderNameWidth()
        {
            return Mathf.Max(CalcLabelSize(T.TagRoleLabel(EffectiveAName)).x, CalcLabelSize(T.TagRoleLabel(EffectiveBName)).x);
        }

        static Vector2 CalcLabelSize(string label)
        {
            return EditorStyles.label.CalcSize(new GUIContent(label));
        }

        Vector2 CalcContactSyncPartsHeaderLabelSize(string label)
        {
            return ContactSyncPartsHeaderLabelStyle.CalcSize(new GUIContent(label));
        }

        float padding = 3;

        bool DrawContactSyncPartsHeader(ContactSyncParts part, float senderNameWidth)
        {
            var rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.IndentedRect(rect);
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var comSpecificWidth = part is ContactSyncCommunicator ? 32 + padding + senderNameWidth : 0;
            rect.width -= comSpecificWidth == 0 ? 0 : comSpecificWidth + padding;
            var exists = DrawContactSyncPartsHeader(rect, part);
            if (part is ContactSyncCommunicator)
            {
                rect.x += rect.width + padding;
                rect.width = comSpecificWidth;
                DrawContactSyncCommunicatorSpecificHeader(rect, part as ContactSyncCommunicator, senderNameWidth);
            }
            EditorGUI.indentLevel = indentLevel;
            return exists;
        }

        bool DrawContactSyncPartsHeader(Rect rect, ContactSyncParts part)
        {
            var dirname = Util.RelativeDirname((target as ContactSyncAssign).transform, part.transform, true);
            var dirnameWidth = 0f;
            if (dirname != "")
            {
                var dirnameRect = rect;
                dirnameWidth = dirnameRect.width = CalcContactSyncPartsHeaderLabelSize(dirname).x;
                EditorGUI.LabelField(dirnameRect, dirname, ContactSyncPartsHeaderLabelStyle);
                rect.x += dirnameWidth;
            }
            rect.width -= 20 + 20 + padding * 2 + dirnameWidth;
            EditorGUI.BeginChangeCheck();
            var name = EditorGUI.TextField(rect, part.gameObject.name, ContactSyncPartsHeaderLabelStyle);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(part.gameObject, "Change Name");
                part.gameObject.name = name;
            }

            rect.x += rect.width + padding;
            rect.width = 20;
            if (GUI.Button(rect, EditorGUIUtility.IconContent("d_search_icon")))
            {
                EditorGUIUtility.PingObject(part);
            }

            rect.x += rect.width + padding;
            rect.width = 20;
            var toRemove = GUI.Button(rect, EditorGUIUtility.IconContent("Toolbar Minus"));
            if (toRemove)
            {
                Undo.DestroyObjectImmediate(part.gameObject);
            }
            return !toRemove;
        }

        void DrawContactSyncCommunicatorSpecificHeader(Rect rect, ContactSyncCommunicator com, float senderNameWidth)
        {
            rect.width = 32;
            if (com is ContactSyncSender)
            {
                DrawSend(rect);
            }
            else
            {
                DrawRecv(rect);
            }
            rect.x += rect.width + padding;
            var tag = Tags.FirstOrDefault(t => t.Name == com.TagName);
            if (tag != null)
            {
                var tagRoleName = com.IsFor(tag) == TagRole.A ? EffectiveAName : EffectiveBName;
                rect.width = senderNameWidth;
                EditorGUI.LabelField(rect, T.TagRoleLabel(tagRoleName));
                rect.x += rect.width + padding;
            }
        }

        void DrawSend(Rect rect)
        {
            var normalColor = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = Color.red;
            EditorGUI.LabelField(rect, T.Send);
            EditorStyles.label.normal.textColor = normalColor;
        }

        void DrawRecv(Rect rect)
        {
            var normalColor = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = Color.cyan;
            EditorGUI.LabelField(rect, T.Recv);
            EditorStyles.label.normal.textColor = normalColor;
        }

        void UpdateParts()
        {
            var contactSyncParts = (target as ContactSyncAssign).GetComponentsInChildren<ContactSyncParts>();
            if (!Enumerable.SequenceEqual(ContactSyncParts, contactSyncParts))
            {
                ContactSyncPartsEditors.Clear();
            }
            ContactSyncParts = contactSyncParts;
        }

        void CreateLackAssigns(TagRole tagRole)
        {
            var parentTransform = (target as ContactSyncAssign).transform;
            var tagAssignInfo = new TagAssignInfo(Tags, ContactSyncParts);
            var lackTags = tagRole == TagRole.A ? tagAssignInfo.ALackTags : tagAssignInfo.BLackTags;
            foreach (var tag in lackTags)
            {
                var isSender = tagRole == tag.Sender;
                var go = new GameObject(isSender ? tag.Name : $"{tag.Name} receiver");
                go.transform.SetParent(parentTransform, false);
                ContactSyncCommunicator com = isSender ? go.AddComponent<ContactSyncSender>() : go.AddComponent<ContactSyncReceiver>();
                com.TagName = tag.Name;
                Undo.RegisterCreatedObjectUndo(go, $"Create {(isSender ? nameof(ContactSyncSender) : nameof(ContactSyncReceiver))}");
            }
        }

        void RemoveUnassigned()
        {
            var tagAssignInfo = new TagAssignInfo(Tags, ContactSyncParts);
            foreach (var item in tagAssignInfo.UnassignedItems)
            {
                Undo.DestroyObjectImmediate(item.gameObject);
            }
        }

        class TagAssignInfo
        {
            public readonly Tag[] Tags;
            public readonly ContactSyncParts[] ContactSyncParts;
            public readonly HashSet<Tag> AAssignedTags;
            public readonly HashSet<Tag> BAssignedTags;
            public readonly List<ContactSyncCommunicator> UnassignedItems;

            public TagAssignInfo(Tag[] tags, ContactSyncParts[] contactSyncParts)
            {
                Tags = tags;
                ContactSyncParts = contactSyncParts;

                var tagsByName = new Dictionary<string, Tag>();
                foreach (var tag in Tags)
                {
                    tagsByName[tag.Name] = tag;
                }
                AAssignedTags = new HashSet<Tag>();
                BAssignedTags = new HashSet<Tag>();
                UnassignedItems = new List<ContactSyncCommunicator>();
                foreach (var parts in ContactSyncParts)
                {
                    var com = parts as ContactSyncCommunicator;
                    if (com == null) continue;
                    if (tagsByName.TryGetValue(com.TagName, out var tag))
                    {
                        if (com.IsFor(tag) == TagRole.A)
                        {
                            AAssignedTags.Add(tag);
                        }
                        else
                        {
                            BAssignedTags.Add(tag);
                        }
                    }
                    else
                    {
                        UnassignedItems.Add(com);
                    }
                }
            }

            public int AAssignedCount => AAssignedTags.Count;
            public int BAssignedCount => BAssignedTags.Count;
            public int UnassignedCount => UnassignedItems.Count;
            public int ALackCount => Tags.Length - AAssignedCount;
            public int BLackCount => Tags.Length - BAssignedCount;
            public IEnumerable<Tag> ALackTags => Tags.Where(tag => !AAssignedTags.Contains(tag));
            public IEnumerable<Tag> BLackTags => Tags.Where(tag => !BAssignedTags.Contains(tag));
        }

        static class T
        {
            public static istring ContactSyncTagGroup = new("Contact Sync Tag Group", "同期タググループ");
            public static istring AllowSelfContact = new("Enable Allow Self (for Debug)", "Allow Self を ON (デバッグ用)");
            public static istring New = new("New", "新規作成");
            public static istring Edit = new("Edit", "編集");
            public static istring AddMAMenuInstaller = new("Add MA Menu Installer", "MA Menu Installer を追加");
            public static istring AddMAMenuGroup = new("Add MA Menu Group", "MA Menu Group を追加");
            public static istring AddAllToggle = new("Add Overall Toggle functionality", "全体ON/OFF機能を追加");
            public static istring AddMatchKey = new("Add Match Key functionality", "マッチングキー機能を追加");
            public static istring CreateAssigns(string sender, int count) => new($"Create assigns of [{sender}] ({count})", $"[{sender}]の割り当てを作る ({count})");
            public static istring RemoveUnassigned = new("Remove unassigned items", "割り当てられていないものを削除");
            public static istring ShowContactSyncParts = new("Items", "アイテム一覧");
            public static istring Send = new("Send", "送信");
            public static istring Recv = new("Recv", "受信");
            public static istring TagRoleLabel(string sender) => new($"({sender})", $"({sender})");
        }
    }
}
