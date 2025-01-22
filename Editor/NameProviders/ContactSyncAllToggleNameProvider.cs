namespace Narazaka.VRChat.ContactSync.Editor.NameProvider
{
    class ContactSyncAllToggleNameProvider : BaseNameProvider<ContactSyncAllToggle>
    {
        internal readonly string ParameterName;

        public ContactSyncAllToggleNameProvider(ContactSyncAllToggle component) : base(component)
        {
            ParameterName = Name(nameof(Component.Enabled));
        }

        internal override string Name(string name) => base.Name($"AllToggle_{name}");
    }
}
