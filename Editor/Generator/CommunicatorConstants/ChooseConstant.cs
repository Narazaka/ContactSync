using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class ChooseConstant
    {
        Constant Constant { get; }

        public ChooseConstant(Constant constant)
        {
            Constant = constant;
        }
        public int MaxChoiceCount => Constant.ValueResolution;

        public float ContactValuePrecision => Constant.ContactValuePrecision;
        public float ContactValueStep => Constant.ContactValueMargin;

        public Vector3 Step => Constant.Margin;
        public Constant.ConstantPosition Min => Constant.Min;
        public Constant.ConstantPosition Max => Constant.Max;
    }
}
