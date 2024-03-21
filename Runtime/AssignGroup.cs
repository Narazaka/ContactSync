using System;

namespace net.narazaka.vrchat.contact_sync
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
