using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class ChooseConstant
    {
        internal const int MaxChoiceCount = 16;
        // | | . | . | . | |
        internal static Vector3 Step = (Constant.MostMaxPosition - Constant.MostMinPosition) / (MaxChoiceCount + 1);
        internal static Vector3 MinEdgePosition = Constant.MostMinPosition + Step;
        internal static Vector3 MaxEdgePosition = Constant.MostMaxPosition - Step;
        internal static Vector3 MinCenterPosition = Constant.EdgeToCenter(MinEdgePosition);

        internal static float MinContactValue = Constant.EdgePositionToContactValue(MinEdgePosition);
        internal static float MaxContactValue = Constant.EdgePositionToContactValue(MaxEdgePosition);
        internal static float ContactValueStep = (MaxContactValue - MinContactValue) / (MaxChoiceCount - 1);
        internal static float ContactValuePrecision = ContactValueStep / 2;
    }
}
