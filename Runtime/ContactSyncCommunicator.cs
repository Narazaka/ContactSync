using System.Linq;
using UnityEngine;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync
{
    [DisallowMultipleComponent]
    public abstract class ContactSyncCommunicator : ContactSyncParts
    {
        [SerializeField] public string TagName;

        public abstract TagRole IsFor(Tag tag);

        public string EffectiveTagBase(ContactSyncTagGroup group)
        {
            var fullTagName = $"{group.Name}{group.Token}{TagName}";
            return group.EncryptTag ? CryptoUtil.Hash(fullTagName) : fullTagName;
        }
    }
}
