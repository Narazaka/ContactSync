using System;
using UnityEngine;

namespace Narazaka.VRChat.ContactSync
{
    [Serializable]
    public class Tag
    {
        [Tooltip("Tag base name")]
        public string Name;
        [Tooltip("Receiver type")]
        public ContactSyncReceiverType ReceiverType = ContactSyncReceiverType.Toggle;
        public float MinVelocity = 0.05f;
        [Tooltip("Memo (does not affect runtime)")]
        public string Memo;
        [Tooltip("Send by")]
        public TagRole SendBy;
        public bool NotifyExists = true;
        public bool Locked = true; // Driverが一発か拘束するか
        public bool AllowUnconstrained = true; // 無指定状態があるかどうか
    }
}
