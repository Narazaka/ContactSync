using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync
{
    public class ContactSyncReceiver : ContactSyncCommunicator
    {
        [SerializeField] public string ParameterName;
        [SerializeField] public bool LocalOnly = true;
        [SerializeField] public bool UseDisableMenu;
        [SerializeField] public bool Enabled = true;
        [SerializeField] public bool Saved;
        [SerializeField] public MenuItem Menu = new MenuItem();

        public override TagRole IsFor(Tag tag) => tag.Sender == TagRole.A ? TagRole.B : TagRole.A;
    }
}
