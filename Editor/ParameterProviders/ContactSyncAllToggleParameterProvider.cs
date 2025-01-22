using nadena.dev.ndmf;
using System.Collections.Generic;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.ParameterProvider
{
    [ParameterProviderFor(typeof(ContactSyncAllToggle))]
    class ContactSyncAllToggleParameterProvider : BaseParameterProvider<ContactSyncAllToggle, NameProvider.ContactSyncAllToggleNameProvider>
    {
        public ContactSyncAllToggleParameterProvider(ContactSyncAllToggle component) : base(component)
        {
        }

        public override IList<VRCExpressionParameters.Parameter> GetParameters()
        {
            if (!NameProvider.Valid) return new VRCExpressionParameters.Parameter[0];
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName,
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = Component.Enabled.ToFloat(),
                    networkSynced = true,
                    saved = Component.Saved,
                },
            };
        }
    }
}
