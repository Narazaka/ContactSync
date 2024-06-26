﻿using System;
using UnityEngine;

namespace net.narazaka.vrchat.contact_sync
{
    [Serializable]
    public class Assign
    {
        public string Name;
        public bool Enabled = true;
        public bool EnableMenu; // runtime切り換え可能にするか
        public string ParameterName;
        public bool LocalOnly = true;
        public string MenuName;
        public Texture2D Icon;
    }
}
