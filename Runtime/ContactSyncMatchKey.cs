using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync
{
    [AddComponentMenu("ContactSync/Parts/ContactSync MatchKey")]
    public class ContactSyncMatchKey : ContactSyncParts
    {
        public const byte MaxValue = 100;
        [SerializeField] public byte MatchKeyA = 50;
        [SerializeField] public byte MatchKeyB = 50;
        [SerializeField] public bool Saved = true;
        [SerializeField] public bool HasParentMenu = true;
        [SerializeField] public bool HasRandomizeMenu = true;
        [SerializeField] public MenuItem ParentMenu = new MenuItem();
        [SerializeField] public MenuItem MatchKeyAMenu = new MenuItem();
        [SerializeField] public MenuItem MatchKeyBMenu = new MenuItem();
        [SerializeField] public MenuItem MatchKeySyncMenu = new MenuItem();
        [SerializeField] public MenuItem MatchKeyRandomizeMenu = new MenuItem();

        public class DefaultMenuName
        {
            public static istring Parent = new("Match Key", "マッチングキー");
            public static istring MatchKeyA = new("Key A", "キーA");
            public static istring MatchKeyB = new("Key B", "キーB");
            public static istring MatchKeySync = new("Apply", "反映");
            public static istring MatchKeyRandomize = new("Apply Random Keys", "ランダムなキーで反映");
        }
    }
}
