using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSyncAllToggle))]
    public class ContactSyncAllToggleEditor : ContactSyncPartsEditor
    {
        SerializedProperty Enabled;
        SerializedProperty Saved;
        SerializedProperty Menu;

        void OnEnable()
        {
            Enabled = serializedObject.FindProperty(nameof(ContactSyncAllToggle.Enabled));
            Saved = serializedObject.FindProperty(nameof(ContactSyncAllToggle.Saved));
            Menu = serializedObject.FindProperty(nameof(ContactSyncAllToggle.Menu));
        }

        public override void OnInspectorGUI()
        {
            UpdateVars();

            DrawBaseInfo();
            DrawMenuInfo();

            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(Enabled, T.InitialEnabled.GUIContent);
            EditorGUILayout.PropertyField(Saved, T.Saved.GUIContent);
            MenuItemDrawer.PropertyField(Menu, T.Menu.GUIContent, ContactSyncAllToggle.DefaultMenuName.Menu);

            serializedObject.ApplyModifiedProperties();
        }

        static class T
        {
            public static istring InitialEnabled => new("Initial Enabled", "‰ŠúON");
            public static istring Saved => new("Saved", "•Û‘¶‚·‚é");
            public static istring Menu => new("Menu", "ƒƒjƒ…[");
        }
    }
}
