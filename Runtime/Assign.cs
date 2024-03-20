﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;

namespace net.narazaka.vrchat.contact_sync
{
    [Serializable]
    public class Assign
    {
        [SerializeField]
        public bool IsSend = true;
        [SerializeField]
        public bool IsReceive;
        [SerializeField]
        public string Name;
        [SerializeField]
        public string ParameterName;
        [SerializeField]
        public bool LocalOnly;
    }
}