using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Narazaka.VRChat.AvatarParametersUtil.Editor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSyncReceiver))]
    public class ContactSyncReceiverEditor : ContactSyncCommunicatorEditor
    {
        AvatarParametersUtilEditor AvatarParametersUtilEditor;
        ContactSyncReceiver ContactSyncReceiver;
        SerializedProperty ParameterName;
        SerializedProperty LocalOnly;
        SerializedProperty UseDisableMenu;
        SerializedProperty Enabled;
        SerializedProperty Saved;
        SerializedProperty Menu;

        protected override sealed void OnEnable()
        {
            base.OnEnable();

            AvatarParametersUtilEditor = AvatarParametersUtilEditor.Get(serializedObject);
            ContactSyncReceiver = target as ContactSyncReceiver;
            ParameterName = serializedObject.FindProperty(nameof(ContactSyncReceiver.ParameterName));
            LocalOnly = serializedObject.FindProperty(nameof(ContactSyncReceiver.LocalOnly));
            UseDisableMenu = serializedObject.FindProperty(nameof(ContactSyncReceiver.UseDisableMenu));
            Enabled = serializedObject.FindProperty(nameof(ContactSyncReceiver.Enabled));
            Saved = serializedObject.FindProperty(nameof(ContactSyncReceiver.Saved));
            Menu = serializedObject.FindProperty(nameof(ContactSyncReceiver.Menu));
        }

        public override void OnInspectorGUI()
        {
            UpdateVars();
            serializedObject.Update();

            DrawBaseInfo();
            DrawMenuInfo();
            DrawTagName();
            AvatarParametersUtilEditor.ShowParameterNameField(EditorGUILayout.GetControlRect(), ParameterName, T.ParameterName.GUIContent);
            EditorGUILayout.PropertyField(LocalOnly, T.LocalOnly.GUIContent);
            EditorGUILayout.PropertyField(UseDisableMenu, T.UseDisableMenu.GUIContent);
            if (UseDisableMenu.boolValue)
            {
                EditorGUILayout.PropertyField(Enabled, T.Enabled.GUIContent);
                EditorGUILayout.PropertyField(Saved, T.Saved.GUIContent);
                DrawMenuInfo();
                MenuItemDrawer.PropertyField(Menu, T.Menu.GUIContent, target.name);
            }

            serializedObject.ApplyModifiedProperties();
        }

        static HashSet<ContactSyncReceiverType> AllowUnconstrainedTypes = new() { ContactSyncReceiverType.Toggle, ContactSyncReceiverType.Choose, ContactSyncReceiverType.Radial };

        static class T
        {
            public static istring ParameterName => new("Parameter Name", "パラメータ名");
            public static istring LocalOnly => new("Local Only", "ローカルのみ");
            public static istring UseDisableMenu => new("Use Disable Menu", "無効化メニューを使用");
            public static istring Enabled => new("Default Enabled", "デフォルト有効");
            public static istring Saved => new("Saved", "保存する");
            public static istring Menu => new("Menu", "メニュー");
        }
    }
}
