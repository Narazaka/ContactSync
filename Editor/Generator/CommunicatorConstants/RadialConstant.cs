namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class RadialConstant
    {
        Constant Constant { get; }

        public RadialConstant(Constant constant)
        {
            Constant = constant;
        }

        public float ContactValuePrecision => Constant.ContactValuePrecision;
        public Constant.ConstantPosition Min => Constant.Min;
        public Constant.ConstantPosition Max => Constant.Max;
    }
}
