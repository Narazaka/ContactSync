using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSyncMatchKey))]
    public class ContactSyncMatchKeyEditor : ContactSyncPartsEditor
    {
        SerializedProperty MatchKeyA;
        SerializedProperty MatchKeyB;
        SerializedProperty Saved;
        SerializedProperty HasParentMenu;
        SerializedProperty HasRandomizeMenu;
        SerializedProperty ParentMenu;
        SerializedProperty MatchKeyAMenu;
        SerializedProperty MatchKeyBMenu;
        SerializedProperty MatchKeySyncMenu;
        SerializedProperty MatchKeyRandomizeMenu;

        void OnEnable()
        {
            MatchKeyA = serializedObject.FindProperty(nameof(ContactSyncMatchKey.MatchKeyA));
            MatchKeyB = serializedObject.FindProperty(nameof(ContactSyncMatchKey.MatchKeyB));
            Saved = serializedObject.FindProperty(nameof(ContactSyncMatchKey.Saved));
            HasParentMenu = serializedObject.FindProperty(nameof(ContactSyncMatchKey.HasParentMenu));
            HasRandomizeMenu = serializedObject.FindProperty(nameof(ContactSyncMatchKey.HasRandomizeMenu));
            ParentMenu = serializedObject.FindProperty(nameof(ContactSyncMatchKey.ParentMenu));
            MatchKeyAMenu = serializedObject.FindProperty(nameof(ContactSyncMatchKey.MatchKeyAMenu));
            MatchKeyBMenu = serializedObject.FindProperty(nameof(ContactSyncMatchKey.MatchKeyBMenu));
            MatchKeySyncMenu = serializedObject.FindProperty(nameof(ContactSyncMatchKey.MatchKeySyncMenu));
            MatchKeyRandomizeMenu = serializedObject.FindProperty(nameof(ContactSyncMatchKey.MatchKeyRandomizeMenu));
        }

        public override void OnInspectorGUI()
        {
            UpdateVars();

            DrawBaseInfo();
            DrawMenuInfo();

            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(MatchKeyA, T.InitialMatchKeyA.GUIContent);
            if (MatchKeyA.intValue > ContactSyncMatchKey.MaxValue) MatchKeyA.intValue = ContactSyncMatchKey.MaxValue;
            EditorGUILayout.PropertyField(MatchKeyB, T.InitialMatchKeyB.GUIContent);
            if (MatchKeyB.intValue > ContactSyncMatchKey.MaxValue) MatchKeyB.intValue = ContactSyncMatchKey.MaxValue;
            if (GUILayout.Button(T.RandomizeMatchKey))
            {
                MatchKeyA.intValue = (byte)Random.Range(0, ContactSyncMatchKey.MaxValue + 1);
                MatchKeyB.intValue = (byte)Random.Range(0, ContactSyncMatchKey.MaxValue + 1);
            }
            EditorGUILayout.PropertyField(Saved, T.Saved.GUIContent);
            EditorGUILayout.PropertyField(HasParentMenu, T.HasParentMenu.GUIContent);
            EditorGUILayout.PropertyField(HasRandomizeMenu, T.HasRandomizeMenu.GUIContent);
            if (HasParentMenu.boolValue)
            {
                MenuItemDrawer.PropertyField(ParentMenu, T.ParentMenu.GUIContent, ContactSyncMatchKey.DefaultMenuName.Parent);
            }
            if (HasParentMenu.boolValue) EditorGUI.indentLevel++;
            MenuItemDrawer.PropertyField(MatchKeyAMenu, T.MatchKeyA.GUIContent, ContactSyncMatchKey.DefaultMenuName.MatchKeyA);
            MenuItemDrawer.PropertyField(MatchKeyBMenu, T.MatchKeyB.GUIContent, ContactSyncMatchKey.DefaultMenuName.MatchKeyB);
            MenuItemDrawer.PropertyField(MatchKeySyncMenu, T.MatchKeySync.GUIContent, ContactSyncMatchKey.DefaultMenuName.MatchKeySync);
            if (HasRandomizeMenu.boolValue)
            {
                MenuItemDrawer.PropertyField(MatchKeyRandomizeMenu, T.MatchKeyRandomize.GUIContent, ContactSyncMatchKey.DefaultMenuName.MatchKeyRandomize);
            }
            if (HasParentMenu.boolValue) EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

        static class T
        {
            public static istring InitialMatchKeyA => new("Initial Match Key A", "�����}�b�`���O�L�[A");
            public static istring InitialMatchKeyB => new("Initial Match Key B", "�����}�b�`���O�L�[B");
            public static istring MatchKeyA => new("Match Key A", "�}�b�`���O�L�[A");
            public static istring MatchKeyB => new("Match Key B", "�}�b�`���O�L�[B");
            public static istring RandomizeMatchKey => new("Randomize Initial match key", "�����}�b�`���O�L�[�������_���Ɍ��߂�");
            public static istring Saved => new("Saved", "�ۑ�����");
            public static istring HasParentMenu => new("Has Parent Menu", "�e���j���[�����");
            public static istring HasRandomizeMenu => new("Has Randomize Menu", "�����_�������j���[�����");
            public static istring ParentMenu => new("Parent Menu", "�e���j���[");
            public static istring MatchKeySync = new("Apply", "���f");
            public static istring MatchKeyRandomize = new("Randomize", "�����_����");
        }
    }
}
