using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class RadialConstant
    {
        internal const float ContactValueCountForPrecision = 16;
        internal const float ContactValuePrecision = 1 / (ContactValueCountForPrecision + 1);
        internal static Vector3 Margin = (Constant.MostMaxPosition - Constant.MostMinPosition) * ContactValuePrecision;
        internal static Vector3 MinEdgePosition = Constant.MostMinPosition + Margin;
        internal static Vector3 MaxEdgePosition = Constant.MostMaxPosition - Margin;
        internal static Vector3 MinCenterPosition = Constant.EdgeToCenter(MinEdgePosition);
        internal static Vector3 MaxCenterPosition = Constant.EdgeToCenter(MaxEdgePosition);

        internal static float MinContactValue = Constant.EdgePositionToContactValue(MinEdgePosition);
        internal static float MaxContactValue = Constant.EdgePositionToContactValue(MaxEdgePosition);
        internal static float MinSideInvalidMargin = Constant.EdgePositionToContactValue(MinEdgePosition - Margin / 2);
        internal static float MaxSideInvalidMargin = Constant.EdgePositionToContactValue(MaxEdgePosition + Margin / 2);
    }
}
