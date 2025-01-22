using System;
using UnityEngine;

namespace Narazaka.VRChat.ContactSync
{
    [Serializable]
    public class Tag
    {
        [Tooltip("Tag base name")]
        public string Name;
        public ContactSyncReceiverType ReceiverType = ContactSyncReceiverTypeUtil.DefaultEnum;
        public float MinVelocity = 0.05f;
        public TagRole Sender;
        [Tooltip("Memo (does not affect runtime)")]
        public string Memo;
        // public bool NotifyExists = true; // 初期には過分な実装
        // public bool Locked = true; // Driverが一発か拘束するか メニューの項目なのでここにはいらない send: ボタンかToggleか？ recv:繰り返しか？
    }
}
