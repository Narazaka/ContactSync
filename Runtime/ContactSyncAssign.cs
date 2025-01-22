using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync
{
    [AddComponentMenu("ContactSync/ContactSync Assign")]
    [DisallowMultipleComponent]
    public class ContactSyncAssign : MonoBehaviour, IEditorOnly
    {
        [SerializeField] public ContactSyncTagGroup ContactSyncTagGroup;
        [SerializeField] public bool AllowSelfContact;

        public ContactSyncAllToggle ContactSyncAllToggle => GetComponentsInChildren<ContactSyncAllToggle>().FirstOrDefault();
        public ContactSyncMatchKey ContactSyncMatchKey => GetComponentsInChildren<ContactSyncMatchKey>().FirstOrDefault();
        public ContactSyncCommunicator[] ContactSyncCommunicators => GetComponentsInChildren<ContactSyncCommunicator>();
    }
}
