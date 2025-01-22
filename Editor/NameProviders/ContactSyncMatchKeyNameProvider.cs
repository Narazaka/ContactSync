namespace Narazaka.VRChat.ContactSync.Editor.NameProvider
{
    class ContactSyncMatchKeyNameProvider : BaseNameProvider<ContactSyncMatchKey>
    {
        internal readonly ParameterNames ParameterName;

        internal class ParameterNames
        {
            public string MatchKeyA;
            public string MatchKeyB;
            public string MatchKeyAUI;
            public string MatchKeyBUI;
            public string Sync;
            public string Randomize;
        }

        public ContactSyncMatchKeyNameProvider(ContactSyncMatchKey component) : base(component)
        {
            ParameterName = new ParameterNames
            {
                MatchKeyA = Name(nameof(ParameterNames.MatchKeyA)),
                MatchKeyB = Name(nameof(ParameterNames.MatchKeyB)),
                MatchKeyAUI = Name(nameof(ParameterNames.MatchKeyAUI)),
                MatchKeyBUI = Name(nameof(ParameterNames.MatchKeyBUI)),
                Sync = Name(nameof(ParameterNames.Sync)),
                Randomize = Name(nameof(ParameterNames.Randomize)),
            };
        }

        internal override string Name(string name) => base.Name($"MatchKey_{name}");
    }
}
