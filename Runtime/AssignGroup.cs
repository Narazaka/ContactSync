using System;

namespace Narazaka.VRChat.ContactSync
{
    [Serializable]
    public class AssignGroup
    {
        public ContactSyncTagGroup ContactSyncTagGroup;
        public bool IsCommander;
        public bool IsFollower;
        public Assign[] CommanderAssigns;
        public Assign[] FollowerAssigns;
    }
}
