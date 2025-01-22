using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor.NameProvider
{
    abstract class BaseNameProvider<T> where T : Component
    {
        public readonly T Component;

        public BaseNameProvider(T component)
        {
            Component = component;
        }
        internal bool Valid => ContactSyncAssign != null;
        internal virtual string Name(string name) => $"ContactSync_{RootName}_{name}";
        string RootName => ContactSyncAssign == null ? "UNKNOWN" : ContactSyncAssign.ContactSyncTagGroup.Name;
        protected virtual ContactSyncAssign ContactSyncAssign => Component.transform.GetComponentInParent<ContactSyncAssign>();
    }
}
