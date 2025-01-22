using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narazaka.VRChat.ContactSync
{
    [CreateAssetMenu(menuName = "Contact Sync/Tag Group")]
    public class ContactSyncTagGroup : ScriptableObject
    {
        // ここにある情報が一致すれば疎通可能（ランタイム変更可能なキー以外）
        [SerializeField]
        public string Name;
        [SerializeField]
        public string Token;
        [SerializeField]
        public bool EncryptTag;
        [SerializeField] // memo
        public string AName;
        public string EffectiveAName => string.IsNullOrEmpty(AName) ? "A" : AName;
        [SerializeField] // memo
        public string BName;
        public string EffectiveBName => string.IsNullOrEmpty(BName) ? "B" : BName;
        [SerializeField]
        public Tag[] Tags;
    }
}
