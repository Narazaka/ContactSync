using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;

namespace net.narazaka.vrchat.contact_sync
{
    [Serializable]
    public class AssignGroup
    {
        [SerializeField]
        public ContactSyncTagGroup ContactSyncTagGroup;
        [SerializeField]
        public Assign[] Assigns;
    }
}
