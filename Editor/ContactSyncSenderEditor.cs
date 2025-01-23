using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomEditor(typeof(ContactSyncSender))]
    public class ContactSyncSenderEditor : ContactSyncCommunicatorEditor
    {
        ContactSyncSender ContactSyncSender;
        DrawMenuHandler _drawMenuHandler;

        protected override sealed void OnEnable()
        {
            base.OnEnable();

            ContactSyncSender = target as ContactSyncSender;
            _drawMenuHandler = new DrawMenuHandler(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            UpdateVars();
            serializedObject.Update();

            DrawBaseInfo();
            DrawTagName();
            if (Tag != null)
            {
                DrawMenuInfo();
                _drawMenuHandler.OnInspectorGUI(Tag.ReceiverType);
            }

            serializedObject.ApplyModifiedProperties();
        }

        class DrawMenuHandler
        {
            SerializedObject SerializedObject;
            SerializedProperty AllowUnconstrained;
            SerializedProperty DefaultValue;
            SerializedProperty Saved;
            SerializedProperty HasParentMenu;
            SerializedProperty ParentMenu;
            SerializedProperty UseToggleButton;
            SerializedProperty UseButton;
            SerializedProperty Menu;
            SerializedProperty OnMenu;
            SerializedProperty OffMenu;
            SerializedProperty ChooseMenus;
            SerializedProperty RadialMenu;
            ContactSyncSender.DefaultMenuName DefaultMenuName;

            public DrawMenuHandler(SerializedObject serializedObject)
            {
                SerializedObject = serializedObject;
                DefaultMenuName = new ContactSyncSender.DefaultMenuName(SerializedObject.targetObject.name);
                AllowUnconstrained = serializedObject.FindProperty(nameof(ContactSyncSender.AllowUnconstrained));
                DefaultValue = serializedObject.FindProperty(nameof(ContactSyncSender.DefaultValue));
                Saved = serializedObject.FindProperty(nameof(ContactSyncSender.Saved));
                HasParentMenu = serializedObject.FindProperty(nameof(ContactSyncSender.HasParentMenu));
                UseToggleButton = serializedObject.FindProperty(nameof(ContactSyncSender.UseToggleButton));
                UseButton = serializedObject.FindProperty(nameof(ContactSyncSender.UseButton));
                ParentMenu = serializedObject.FindProperty(nameof(ContactSyncSender.ParentMenu));
                Menu = serializedObject.FindProperty(nameof(ContactSyncSender.Menu));
                OnMenu = serializedObject.FindProperty(nameof(ContactSyncSender.OnMenu));
                OffMenu = serializedObject.FindProperty(nameof(ContactSyncSender.OffMenu));
                ChooseMenus = serializedObject.FindProperty(nameof(ContactSyncSender.ChooseMenus));
                RadialMenu = serializedObject.FindProperty(nameof(ContactSyncSender.RadialMenu));
            }

            public void OnInspectorGUI(ContactSyncReceiverType receiverType)
            {
                DefaultMenuName.BaseName = SerializedObject.targetObject.name;

                if (AllowUnconstrainedTypes.Contains(receiverType))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(AllowUnconstrained, T.AllowUnconstrained.GUIContent);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (AllowUnconstrained.boolValue)
                        {
                            DefaultValue.floatValue = -1;
                        }
                    }
                }
                DrawDefaultValue(receiverType);
                if (receiverType != ContactSyncReceiverType.Trigger)
                {
                    EditorGUILayout.PropertyField(Saved, T.Saved.GUIContent);
                }
                DrawMenus(receiverType);
            }

            void DrawDefaultValue(ContactSyncReceiverType receiverType)
            {
                if (receiverType == ContactSyncReceiverType.Trigger)
                {
                    DrawButtonType(UseToggleButton, T.Button.GUIContent, T.ToggleButton.GUIContent);
                    return;
                }

                bool unconstrained = false;
                if (AllowUnconstrained.boolValue)
                {
                    unconstrained = EditorGUILayout.Toggle(T.DefaultUnconstrained, DefaultValue.floatValue < 0);
                    if (unconstrained)
                    {
                        DefaultValue.floatValue = -1;
                    }
                }
                if (unconstrained)
                {
                    if (receiverType == ContactSyncReceiverType.Toggle || receiverType == ContactSyncReceiverType.Choose)
                    {
                        DrawButtonType(UseButton, T.ToggleButton.GUIContent, T.Button.GUIContent);
                    }
                    return;
                }
                if (DefaultValue.floatValue < 0)
                {
                    DefaultValue.floatValue = 0;
                }
                switch (receiverType)
                {
                    case ContactSyncReceiverType.Toggle:
                        DefaultValue.floatValue = EditorGUILayout.Toggle(T.DefaultValue, DefaultValue.floatValue.ToBool()).ToFloat();
                        break;
                    case ContactSyncReceiverType.Choose:
                        DefaultValue.floatValue = EditorGUILayout.IntField(T.DefaultValue, (int)DefaultValue.floatValue);
                        break;
                    case ContactSyncReceiverType.Radial:
                        EditorGUILayout.PropertyField(DefaultValue, T.DefaultValue.GUIContent);
                        break;
                }
            }

            void DrawMenus(ContactSyncReceiverType receiverType)
            {
                switch (receiverType)
                {
                    case ContactSyncReceiverType.Constant:
                    case ContactSyncReceiverType.OnEnter:
                    case ContactSyncReceiverType.Trigger:
                        DrawMenu();
                        break;
                    case ContactSyncReceiverType.Toggle:
                        if (AllowUnconstrained.boolValue)
                        {
                            var hasParent = DrawParentMenu();
                            if (hasParent) EditorGUI.indentLevel++;
                            DrawOnOffMenu();
                            if (hasParent) EditorGUI.indentLevel--;
                        }
                        else
                        {
                            DrawMenu();
                        }
                        break;
                    case ContactSyncReceiverType.Choose:
                        DrawParentMenu();
                        DrawChooseMenus();
                        break;
                    case ContactSyncReceiverType.Proximity:
                        DrawRadialMenu();
                        break;
                    case ContactSyncReceiverType.Radial:
                        if (AllowUnconstrained.boolValue)
                        {
                            var hasParent = DrawParentMenu();
                            if (hasParent) EditorGUI.indentLevel++;
                            DrawRadialOnMenu();
                            DrawRadialMenu();
                            if (hasParent) EditorGUI.indentLevel--;
                        }
                        else
                        {
                            DrawRadialMenu();
                        }
                        break;
                }
            }

            bool DrawParentMenu()
            {
                EditorGUILayout.PropertyField(HasParentMenu, T.HasParentMenu.GUIContent);
                if (HasParentMenu.boolValue)
                {
                    MenuItemDrawer.PropertyField(ParentMenu, T.ParentMenu.GUIContent, DefaultMenuName.ParentMenu);
                }
                return HasParentMenu.boolValue;
            }

            void DrawMenu()
            {
                MenuItemDrawer.PropertyField(Menu, new GUIContent("Menu"), DefaultMenuName.Menu);
            }

            void DrawOnOffMenu()
            {
                MenuItemDrawer.PropertyField(OnMenu, new GUIContent("ON"), DefaultMenuName.OnMenu);
                MenuItemDrawer.PropertyField(OffMenu, new GUIContent("OFF"), DefaultMenuName.OffMenu);
            }

            void DrawChooseMenus()
            {
                EditorGUILayout.PropertyField(ChooseMenus, true);
            }

            void DrawRadialMenu()
            {
                MenuItemDrawer.PropertyField(RadialMenu, new GUIContent("Radial"), DefaultMenuName.RadialMenu);
            }

            void DrawRadialOnMenu()
            {
                MenuItemDrawer.PropertyField(OnMenu, new GUIContent("ON"), DefaultMenuName.OnMenu);
            }

            void DrawButtonType(SerializedProperty useAlternateButton, GUIContent defaultButton, GUIContent alternateButton)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(T.ButtonType.GUIContent, GUILayout.Width(EditorGUIUtility.labelWidth));
                useAlternateButton.boolValue = GUILayout.Toolbar(useAlternateButton.boolValue ? 1 : 0, new GUIContent[] { defaultButton, alternateButton }) != 0;
                EditorGUILayout.EndHorizontal();
            }
        }

        static HashSet<ContactSyncReceiverType> AllowUnconstrainedTypes = new() { ContactSyncReceiverType.Toggle, ContactSyncReceiverType.Choose, ContactSyncReceiverType.Radial };

        static class T
        {
            public static istring DefaultUnconstrained => new("Default Unconstrained", "デフォルト無指定");
            public static istring DefaultValue => new("Default Value", "デフォルト値");
            public static istring AllowUnconstrained => new("Allow Unconstrained", "無指定を可能に");
            public static istring Saved => new("Saved", "保存する");
            public static istring HasParentMenu => new("Has Parent Menu", "親メニューを作る");
            public static istring ButtonType => new("Button Type", "ボタンタイプ");
            public static istring ToggleButton => new("Toggle Button", "トグルボタン");
            public static istring Button => new("Button (One Shot)", "ボタン（単押し）");
            public static istring ParentMenu => new("Parent Menu", "親メニュー");
            public static istring ChoiceNames => new("Choice Names", "選択肢名");
        }
    }
}
