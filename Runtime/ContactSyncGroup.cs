using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync
{
    public class ContactSyncGroup : MonoBehaviour, IEditorOnly
    {
        [SerializeField] public string Token; // password
        [SerializeField] public bool EncryptTag;
        [SerializeField] public byte MatchKeyA = 128;
        [SerializeField] public byte MatchKeyB = 128;
    }
}
