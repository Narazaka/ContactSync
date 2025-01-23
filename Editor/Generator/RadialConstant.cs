using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class RadialConstant
    {
        internal static Vector3 MinEdgePosition = Constant.Direction * 0.9f;
        internal static Vector3 MaxEdgePosition = Constant.Direction * 0.1f;
        internal static Vector3 MinCenterPosition = Constant.EdgeToCenter(MinEdgePosition);
        internal static Vector3 MaxCenterPosition = Constant.EdgeToCenter(MaxEdgePosition);

        internal static float MinContactValue = Constant.EdgePositionToContactValue(MinEdgePosition);
        internal static float MaxContactValue = Constant.EdgePositionToContactValue(MaxEdgePosition);
    }
}
