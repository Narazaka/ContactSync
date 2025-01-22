using UnityEngine;
using nadena.dev.modular_avatar.core;

namespace Narazaka.VRChat.ContactSync
{
    public class ContactSyncSender : ContactSyncCommunicator
    {
        [SerializeField] public bool AllowUnconstrained = true; // 無指定状態があるかどうか
        [SerializeField] public float DefaultValue = -1;
        [SerializeField] public bool Saved;
        [SerializeField] public bool HasParentMenu = true;
        [SerializeField] public MenuItem ParentMenu = new MenuItem();
        [SerializeField] public MenuItem Menu = new MenuItem();
        [SerializeField] public MenuItem OnMenu = new MenuItem();
        [SerializeField] public MenuItem OffMenu = new MenuItem();
        [SerializeField] public MenuItem[] ChooseMenus = new MenuItem[0];
        [SerializeField] public MenuItem RadialMenu = new MenuItem();

        public override TagRole IsFor(Tag tag) => tag.Sender;

        public class DefaultMenuName
        {
            public string BaseName { get; set; }
            public DefaultMenuName(string name)
            {
                BaseName = name;
            }
            public string ParentMenu => Menu;
            public string Menu => BaseName;
            public string OnMenu => $"{BaseName} ON";
            public string OffMenu => $"{BaseName} OFF";
            public string ChooseMenu(int index) => $"{BaseName} {index + 1}";
            public string RadialMenu => Menu;
        }
    }
}
