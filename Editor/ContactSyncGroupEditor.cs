using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSyncGroup))]
    class ContactSyncGroupEditor : UnityEditor.Editor
    {
        SerializedProperty Token;
        SerializedProperty EncryptTag;
        SerializedProperty MatchKeyA;
        SerializedProperty MatchKeyB;

        void OnEnable()
        {
            Token = serializedObject.FindProperty(nameof(ContactSyncGroup.Token));
            EncryptTag = serializedObject.FindProperty(nameof(ContactSyncGroup.EncryptTag));
            MatchKeyA = serializedObject.FindProperty(nameof(ContactSyncGroup.MatchKeyA));
            MatchKeyB = serializedObject.FindProperty(nameof(ContactSyncGroup.MatchKeyB));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(Token, T.Token.GUIContent);

            if (GUILayout.Button(T.RandomizeToken))
            {
                Token.stringValue = System.Guid.NewGuid().ToString();
            }

            EditorGUILayout.PropertyField(EncryptTag, T.EncryptTag.GUIContent);
            EditorGUILayout.PropertyField(MatchKeyA, T.MatchKeyA.GUIContent);
            EditorGUILayout.PropertyField(MatchKeyB, T.MatchKeyB.GUIContent);

            if (GUILayout.Button(T.RandomizeMatchKey))
            {
                MatchKeyA.intValue = (byte)Random.Range(0, 256);
                MatchKeyB.intValue = (byte)Random.Range(0, 256);
            }

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button(T.CreateAllToggleMenu))
            {
                // TODO: Menu/ContactSyncEnableMenuSet create
            }
        }

        static class T
        {
            public static istring Token => new("Token", "トークン");
            public static istring RandomizeToken => new("Randomize Token", "トークンをランダムに決める");
            public static istring EncryptTag => new("Encrypt Tag", "タグ暗号化");
            public static istring MatchKeyA => new("Match Key A", "マッチキーA");
            public static istring MatchKeyB => new("Match Key B", "マッチキーB");
            public static istring RandomizeMatchKey => new("Randomize Initial match key", "初期マッチキーをランダムに決める");
            public static istring CreateUnconstrainedToggleMenu => new("Create Toggle Menu (unconstrained)", "ON/OFFメニューを作成 (非拘束状態あり)");
            public static istring CreateAllToggleMenu => new("Create Enable/Disable Menu", "全体ON/OFFメニューを作成");
        }
    }
}
