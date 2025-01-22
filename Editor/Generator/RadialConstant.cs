using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class RadialConstant
    {
        internal static Vector3 MinEdgePosition = Vector3.down * 0.9f;
        internal static Vector3 MaxEdgePosition = Vector3.zero;
        internal static Vector3 MinCenterPosition = MinEdgePosition + Vector3.down * Constant.SenderContactRadius;
        internal static Vector3 MaxCenterPosition = MaxEdgePosition + Vector3.down * Constant.SenderContactRadius;

        internal static Vector3 EdgePosition = Vector3.down;
        internal static float MinContactValue = MinEdgePosition.y - EdgePosition.y;
        internal const float MaxContactValue = 1f;
    }
}
