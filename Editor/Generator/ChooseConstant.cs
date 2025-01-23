using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class ChooseConstant
    {
        internal static Vector3 MinEdgePosition = Constant.Direction * 0.9f;
        internal static Vector3 MaxEdgePosition = Constant.Direction * 0.1f;
        internal static Vector3 MinCenterPosition = Constant.EdgeToCenter(MinEdgePosition);
        internal const int MaxChoiceCount = 16;
        internal static Vector3 Step = (MaxEdgePosition - MinEdgePosition) / (MaxChoiceCount - 1);

        internal static float MinContactValue = Constant.EdgePositionToContactValue(MinEdgePosition);
        internal static float MaxContactValue = Constant.EdgePositionToContactValue(MaxEdgePosition);
        internal static float ContactValueStep = (MaxContactValue - MinContactValue) / (MaxChoiceCount - 1);
        internal static float HalfContactValueStep = ContactValueStep / 2;
    }
}
