using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class Constant
    {
        internal const float FloatPrecision = 1 / 255f;
        internal const float SenderContactRadius = FloatPrecision;
        internal static Vector3 Direction = Vector3.down;
        /** should near min */
        internal static Vector3 MostMinPosition = Direction * 1;
        /** should near max */
        internal static Vector3 MostMaxPosition = Vector3.zero;

        // because inner edge makes value
        internal static Vector3 EdgeToCenter(Vector3 edge) => edge + Direction * SenderContactRadius;
        internal static float EdgePositionToContactValue(Vector3 edge) => Vector3.Dot(edge - MostMinPosition, MostMaxPosition - MostMinPosition);
    }
}
