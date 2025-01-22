using UnityEngine;

namespace Narazaka.VRChat.ContactSync
{
    public class ContactSyncAllToggle : ContactSyncParts
    {
        [SerializeField] public bool Enabled = true;
        [SerializeField] public bool Saved;
        [SerializeField] public MenuItem Menu = new MenuItem();

        public class DefaultMenuName
        {
            public static string Menu = "ON";
        }
    }
}
