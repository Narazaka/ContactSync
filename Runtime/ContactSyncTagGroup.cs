using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narazaka.VRChat.ContactSync
{
    [CreateAssetMenu(menuName = "Contact Sync/Tag Group")]
    public class ContactSyncTagGroup : ScriptableObject
    {
        [SerializeField]
        public string Prefix;
        [SerializeField]
        public Tag[] Tags;
    }
}
