using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Narazaka.VRChat.ContactSync.Editor
{
    public abstract class ContactSyncPartsEditor : UnityEditor.Editor
    {
        public bool DrawDetail = true;
        protected ContactSyncAssign ContactSyncAssign;

        protected virtual void UpdateVars()
        {
            ContactSyncAssign = (target as Component).GetComponentInParent<ContactSyncAssign>();
        }

        protected void DrawBaseInfo()
        {
            if (ContactSyncAssign == null)
            {
                EditorGUILayout.HelpBox(T.BaseInfo, MessageType.Warning);
            }
        }

        protected void DrawMenuInfo()
        {
            if (DrawDetail)
            {
                EditorGUILayout.HelpBox(T.MenuInfo, MessageType.Info);
            }
        }

        static class T
        {
            public static istring BaseInfo => new("For this component to work properly, it must be placed within Contact Sync Assign.", "このコンポーネントを正しく動作させるには、Contact Sync Assign 内に配置する必要があります。");
            public static istring MenuInfo => new("This component behaves like the MA Menu Group", "このコンポーネントはMA Menu Groupのように振る舞います");
        }
    }
}
