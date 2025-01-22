using UnityEngine;
using UnityEditor;
using System.Linq;
using VRC.Dynamics;

namespace Narazaka.VRChat.ContactSync.Editor
{
    static class ContactSyncReceiverTypeUtil
    {
        public static ContactReceiver.ReceiverType ToReceiverType(this ContactSyncReceiverType receiverType)
        {
            switch (receiverType)
            {
                case ContactSyncReceiverType.Constant:
                    return ContactReceiver.ReceiverType.Constant;
                case ContactSyncReceiverType.OnEnter:
                    return ContactReceiver.ReceiverType.OnEnter;
                case ContactSyncReceiverType.Proximity:
                    return ContactReceiver.ReceiverType.Proximity;
                case ContactSyncReceiverType.Trigger:
                    return ContactReceiver.ReceiverType.Constant;
                case ContactSyncReceiverType.Toggle:
                    return ContactReceiver.ReceiverType.Constant;
                case ContactSyncReceiverType.Choose:
                    return ContactReceiver.ReceiverType.Constant;
                default:
                    return ContactReceiver.ReceiverType.Proximity;
            }
        }
    }
}
