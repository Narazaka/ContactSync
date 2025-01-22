namespace Narazaka.VRChat.ContactSync.Editor.NameProvider
{
    class ContactSyncReceiverNameProvider : BaseNameProvider<ContactSyncReceiver>
    {
        internal readonly ParameterNames ParameterName;

        internal class ParameterNames
        {
            public string Contact;
            public string Enabled;
        }

        public ContactSyncReceiverNameProvider(ContactSyncReceiver component) : base(component)
        {
            ParameterName = new ParameterNames
            {
                Contact = Name(nameof(ParameterNames.Contact)),
                Enabled = Name(nameof(ParameterNames.Enabled)),
            };
        }

        internal override string Name(string name) => base.Name($"Receiver_{Component.TagName}_{name}");
    }
}
