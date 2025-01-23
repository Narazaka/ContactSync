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
            var parameters = new List<VRCExpressionParameters.Parameter>();
            parameters.AddRange(GetCoreParameters());
            parameters.AddRange(GetUIParameters());
            return parameters;
        }

        public IList<VRCExpressionParameters.Parameter> GetCoreParameters()
        {
            return new List<VRCExpressionParameters.Parameter>
            {
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyA,
                    valueType = VRCExpressionParameters.ValueType.Int,
                    defaultValue = Component.MatchKeyA,
                    networkSynced = true,
                    saved = Component.Saved,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyB,
                    valueType = VRCExpressionParameters.ValueType.Int,
                    defaultValue = Component.MatchKeyB,
                    networkSynced = true,
                    saved = Component.Saved,
                },
            };
        }

        public IList<VRCExpressionParameters.Parameter> GetUIParameters()
        {
            var parameters = new List<VRCExpressionParameters.Parameter> {
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyAInt,
                    valueType = VRCExpressionParameters.ValueType.Int,
                    defaultValue = Component.MatchKeyA,
                    networkSynced = false,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyBInt,
                    valueType = VRCExpressionParameters.ValueType.Int,
                    defaultValue = Component.MatchKeyB,
                    networkSynced = false,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyAFloat,
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = 0,
                    networkSynced = false,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyBFloat,
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = 0,
                    networkSynced = false,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyAUI,
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = Component.MatchKeyA / 100f,
                    networkSynced = false,
                },
                new VRCExpressionParameters.Parameter
                {
                    name = NameProvider.ParameterName.MatchKeyBUI,
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = Component.MatchKeyB / 100f,
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
