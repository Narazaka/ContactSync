namespace Narazaka.VRChat.ContactSync.Editor.NameProvider
{
    class ContactSyncSenderNameProvider : BaseNameProvider<ContactSyncSender>
    {
        internal readonly ParameterNames ParameterName;

        internal class ParameterNames
        {
            public string ON;
            public string OFF;
            public string Value;
        }

        public ContactSyncSenderNameProvider(ContactSyncSender component) : base(component)
        {
            ParameterName = new ParameterNames
            {
                ON = Name(nameof(ParameterNames.ON)),
                OFF = Name(nameof(ParameterNames.OFF)),
                Value = Name(nameof(ParameterNames.Value)),
            };
        }

        internal override string Name(string name) => base.Name($"Sender_{Component.TagName}_{name}");
    }
}
