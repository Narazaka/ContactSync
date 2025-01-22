using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    static class ChooseConstant
    {
        internal static Vector3 MinPosition = Vector3.down * 0.9f;
        internal static Vector3 MaxPosition = Vector3.zero;
        internal const int MaxChoiceCount = 16;
        internal static Vector3 Step = (MaxPosition - MinPosition) / (MaxChoiceCount - 1);

        internal static Vector3 EdgePosition = Vector3.down;
        internal static float MinContactValue = MinPosition.y - EdgePosition.y;
        internal const float MaxContactValue = 1f;
        internal static float ContactValueStep = (MaxContactValue - MinContactValue) / (MaxChoiceCount - 1);
        internal static float HalfContactValueStep = ContactValueStep / 2;
    }
}
