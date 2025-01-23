namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class ConstantSet
    {
        public ToggleConstant Toggle { get; }
        public ChooseConstant Choose { get; }
        public RadialConstant Radial { get; }

        public ConstantSet(int valueResolution)
        {
            Toggle = new ToggleConstant();
            var constant = new Constant(valueResolution);
            Choose = new ChooseConstant(constant);
            Radial = new RadialConstant(constant);
        }
    }
}
