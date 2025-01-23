using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class Constant
    {
        internal const float FloatPrecision = 1 / 255f;
        internal const float SenderContactRadius = FloatPrecision;
        internal static Vector3 Direction = Vector3.down;
        internal static Vector3 FullDirection = Direction * 1;

        internal static Vector3 EdgeToCenter(Vector3 edge) => edge + Direction * SenderContactRadius;
        internal static float EdgePositionToContactValue(Vector3 edge) => 1f - Vector3.Dot(edge, FullDirection);
    }
}
