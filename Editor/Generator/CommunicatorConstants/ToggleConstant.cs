namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class ToggleConstant
    {
        Constant Constant { get; }
        public ToggleConstant()
        {
            Constant = new Constant(2);
        }

        public float ContactValuePrecision => Constant.ContactValuePrecision;

        public Constant.ConstantPosition OFF => Constant.Min;
        public Constant.ConstantPosition ON => Constant.Max;
    }
}
