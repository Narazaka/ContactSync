using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSyncTagGroup))]
    public class ContactSyncTagGroupEditor : UnityEditor.Editor
    {
        SerializedProperty Name;
        SerializedProperty Token;
        SerializedProperty EncryptTag;
        SerializedProperty AName;
        SerializedProperty BName;
        SerializedProperty Tags;
#if !UNITY_2020_3_OR_NEWER
        ReorderableList TagsList;
#endif

        float helpBoxHeight;

        void OnEnable()
        {
            helpBoxHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
            Name = serializedObject.FindProperty(nameof(ContactSyncTagGroup.Name));
            Token = serializedObject.FindProperty(nameof(ContactSyncTagGroup.Token));
            EncryptTag = serializedObject.FindProperty(nameof(ContactSyncTagGroup.EncryptTag));
            AName = serializedObject.FindProperty(nameof(ContactSyncTagGroup.AName));
            BName = serializedObject.FindProperty(nameof(ContactSyncTagGroup.BName));
            Tags = serializedObject.FindProperty(nameof(ContactSyncTagGroup.Tags));
#if !UNITY_2020_3_OR_NEWER
            TagsList = new ReorderableList(serializedObject, Tags);
            TagsList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, T.Tags);
            TagsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => EditorGUI.PropertyField(rect, Tags.GetArrayElementAtIndex(index));
            TagsList.elementHeightCallback = (int index) => EditorGUI.GetPropertyHeight(Tags.GetArrayElementAtIndex(index));
            TagsList.onAddCallback = (ReorderableList list) =>
            {
                Tags.arraySize++;
                if (Tags.arraySize == 1)
                {
                    var tag = Tags.GetArrayElementAtIndex(0);
                    tag.FindPropertyRelative(nameof(Tag.ReceiverType)).enumValueIndex = (int)ContactSyncReceiverType.Toggle;
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

            EditorGUIHelper.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), Name, T.Name.GUIContent, T.NamePlaceholder);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, helpBoxHeight), T.NameDescription, MessageType.Info);
            rect.y += helpBoxHeight + EditorGUIUtility.standardVerticalSpacing;
            var generateTokenWidth = 100;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - generateTokenWidth, EditorGUIUtility.singleLineHeight), Token, T.Token.GUIContent);
            if (GUI.Button(new Rect(rect.x + rect.width - generateTokenWidth, rect.y, generateTokenWidth, EditorGUIUtility.singleLineHeight), T.GenerateToken.GUIContent))
            {
                Token.stringValue = CryptoUtil.RandomHash();
            }
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, helpBoxHeight), T.TokenDescription, MessageType.Info);
            rect.y += helpBoxHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), EncryptTag, T.EncryptTag.GUIContent);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, helpBoxHeight), T.EncryptTagDescription, MessageType.Info);
            rect.y += helpBoxHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUIHelper.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), AName, T.AName.GUIContent, T.ANamePlaceholder);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUIHelper.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), BName, T.BName.GUIContent, T.BNamePlaceholder);
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
#if UNITY_2020_3_OR_NEWER
            EditorGUI.PropertyField(rect, Tags, T.Tags.GUIContent);
#else
            TagsList.DoList(rect);
#endif

            serializedObject.ApplyModifiedProperties();
        }

        public float GetHeight()
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5 + (helpBoxHeight + EditorGUIUtility.standardVerticalSpacing) * 3 +
#if UNITY_2020_3_OR_NEWER
                EditorGUI.GetPropertyHeight(Tags);
#else
                TagsList.GetHeight();
#endif
        }

        static class T
        {
            public static istring Name = new("Name", "名前");
            public static istring NamePlaceholder = new("MyAwesomeGimmick... secret_of_A_and_B...", "MyAwesomeGimmick... AとBの秘密...");
            public static istring NameDescription = new("The base name for VRCContacts. It should be unique and not duplicate other gimmicks.", "VRCContactsのベース名。他のギミックと重複しないユニークなものにしてください。");
            public static istring Token = new("Token", "鍵");
            public static istring TokenDescription = new("Enter if you are using it for a specific small group of people, etc.", "特定少数のグループ等で使う場合に入力してください。");
            public static istring EncryptTag = new("Encrypt Tag", "タグを暗号化");
            public static istring EncryptTagDescription = new("Used when you want to keep the tag string secret from others. It is possible to communicate with the same gimmick encrypted.", "タグ文字列を外部から秘匿したい場合に使います。暗号化した同じギミックとは通信可能です。");
            public static istring GenerateToken = new("Generate Token", "鍵を生成");
            public static istring AName = new("Name of Role A (memo)", "Aの役割名 (メモ)");
            public static istring ANamePlaceholder = new("Commander... A-san...", "指示者... Aさん...");
            public static istring BName = new("Name of Role B (memo)", "Bの役割名 (メモ)");
            public static istring BNamePlaceholder = new("Follower... B-san...", "従属者... Bさん...");
            public static istring Tags = new("Tags", "タグ一覧");
        }
    }
}
