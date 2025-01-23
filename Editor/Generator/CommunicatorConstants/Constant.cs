using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class Constant
    {
        public const float SenderContactRadius = 1 / 8f;
        public static Vector3 Direction = Vector3.down;
        /** should near min */
        public static Vector3 MostMinPosition = Direction * 1;
        /** should near max */
        public static Vector3 MostMaxPosition = Vector3.zero;

        // because inner edge makes value
        public static Vector3 EdgeToCenter(Vector3 edge) => edge + Direction * SenderContactRadius;
        public static float EdgePositionToContactValue(Vector3 edge) => Vector3.Dot(edge - MostMinPosition, MostMaxPosition - MostMinPosition);

        public readonly struct ConstantPosition
        {
            public readonly Vector3 EdgePosition;
            public readonly Vector3 CenterPosition;
            public readonly float ContactValue;

            public ConstantPosition(Vector3 edge)
            {
                EdgePosition = edge;
                CenterPosition = EdgeToCenter(EdgePosition);
                ContactValue = EdgePositionToContactValue(EdgePosition);
            }
        }

        // ex. ValueResolution = 3
        // => region = 1 /4
        // choose: | | . | . | . | |
        // radial: |   >       <   |
        // toggle: choose.resolusion = 2
        public int ValueResolution { get; }

        public Constant(int valueResolution)
        {
            ValueResolution = valueResolution;
            ContactValueMargin = 1f / (ValueResolution + 1);
            ContactValuePrecision = ContactValueMargin / 2;
            Margin = (MostMaxPosition - MostMinPosition) * ContactValueMargin;
            Min = new ConstantPosition(MostMinPosition + Margin);
            Max = new ConstantPosition(MostMaxPosition - Margin);
        }


        public float ContactValueMargin { get; }
        public float ContactValuePrecision { get; }

        public Vector3 Margin { get; }
        public ConstantPosition Min { get; }
        public ConstantPosition Max { get; }
    }
}
