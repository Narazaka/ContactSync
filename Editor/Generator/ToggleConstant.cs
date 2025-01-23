using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class ToggleConstant
    {
        internal static Vector3 OFFEdgePosition = Constant.Direction * 2 / 3f;
        internal static Vector3 ONEdgePosition = Constant.Direction * 1 / 3f;
        internal static Vector3 OFFCenterPosition = Constant.EdgeToCenter(OFFEdgePosition);
        internal static Vector3 ONCenterPosition = Constant.EdgeToCenter(ONEdgePosition);

        internal static float ContactValuePrecision = 1 / 6f;
        internal static float OFFContactValue = Constant.EdgePositionToContactValue(OFFEdgePosition);
        internal static float ONContactValue = Constant.EdgePositionToContactValue(ONEdgePosition);
    }
}
