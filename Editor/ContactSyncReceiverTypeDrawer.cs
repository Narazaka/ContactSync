using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace Narazaka.VRChat.ContactSync.Editor
{
    [CustomPropertyDrawer(typeof(ContactSyncReceiverType))]
    class ContactSyncReceiverTypeDrawer : PropertyDrawer
    {
        static istring[] Labels = typeof(ContactSyncReceiverType).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Select(item =>
        {
            var istr = item.GetCustomAttribute<IStringAttribute>();
            if (istr != null) return istr.Data;
            return new istring(item.Name, item.Name);
        }).ToArray();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.enumValueIndex = EditorGUI.Popup(position, property.enumValueIndex - ContactSyncReceiverTypeUtil.MinEnum, Labels.Skip(ContactSyncReceiverTypeUtil.MinEnum).Select(l => (string)l).ToArray()) + ContactSyncReceiverTypeUtil.MinEnum;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}
