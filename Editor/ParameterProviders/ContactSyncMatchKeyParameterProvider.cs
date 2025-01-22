using System.Linq;
using nadena.dev.ndmf;
using System.Collections.Generic;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.ParameterProvider
{
    [ParameterProviderFor(typeof(ContactSyncMatchKey))]
    class ContactSyncMatchKeyParameterProvider : BaseParameterProvider<ContactSyncMatchKey, NameProvider.ContactSyncMatchKeyNameProvider>
    {
        public ContactSyncMatchKeyParameterProvider(ContactSyncMatchKey component) : base(component)
        {
        }

        public override IList<VRCExpressionParameters.Parameter> GetParameters()
        {
            return GetDirectControllerParameters().Concat(GetControllerParameters()).ToArray();
        }

        public IList<VRCExpressionParameters.Parameter> GetDirectControllerParameters()
        {
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyA,
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = Component.MatchKeyA / 255f,
                    networkSynced = true,
                    saved = Component.Saved,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyB,
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = Component.MatchKeyB / 255f,
                    networkSynced = true,
                    saved = Component.Saved,
                }
            };
        }

        public IList<VRCExpressionParameters.Parameter> GetControllerParameters()
        {
            var parameters = new List<VRCExpressionParameters.Parameter>
            {
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyAUI,
                    valueType = VRCExpressionParameters.ValueType.Int,
                    defaultValue = Component.MatchKeyA,
                    networkSynced = false,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyBUI,
                    valueType = VRCExpressionParameters.ValueType.Int,
                    defaultValue = Component.MatchKeyB,
                    networkSynced = false,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.Sync,
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = false,
                },
            };
            if (Component.HasRandomizeMenu)
            {
                parameters.Add(new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.Randomize,
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = false,
                });
            }
            return parameters;
        }
    }
}
