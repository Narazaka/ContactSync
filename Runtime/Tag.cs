﻿using System;
using UnityEngine;

namespace net.narazaka.vrchat.contact_sync
{
    [Serializable]
    public class Tag
    {
        [Tooltip("Tag base name")]
        public string Name;
        [Tooltip("Receiver type")]
        public ContactSyncReceiverType ReceiverType;
        public float MinVelocity = 0.05f;
        [Tooltip("Memo (does not affect runtime)")]
        public string Memo;
        public TagRole Role;
        public bool MarkExist = true;
        public bool Continuous = true; // Driverが一発か拘束するか
        public bool Separated = false; // ON/OFFを3値にするか
    }
}
