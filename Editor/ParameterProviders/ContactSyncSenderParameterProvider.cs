using nadena.dev.ndmf;
using System.Collections.Generic;
using System.Linq;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.ParameterProvider
{
    [ParameterProviderFor(typeof(ContactSyncSender))]
    class ContactSyncSenderParameterProvider : BaseParameterProvider<ContactSyncSender, NameProvider.ContactSyncSenderNameProvider>
    {
        NameProvider.ContactSyncSenderNameProvider.ParameterNames ParameterName;
        Tag Tag;

        public ContactSyncSenderParameterProvider(ContactSyncSender component) : base(component)
        {
            ParameterName = NameProvider.ParameterName;
            var assign = Component.GetComponentInParent<ContactSyncAssign>();
            if (assign == null || assign.ContactSyncTagGroup == null)
            {
                return;
            }
            Tag = assign.ContactSyncTagGroup.Tags.FirstOrDefault(tag => tag.Name == Component.TagName);
        }

        public override IList<VRCExpressionParameters.Parameter> GetParameters()
        {
            if (Tag == null) return new VRCExpressionParameters.Parameter[0];
            switch (Tag.ReceiverType)
            {
                case ContactSyncReceiverType.Trigger:
                    return Trigger();
                case ContactSyncReceiverType.Toggle:
                    return Toggle();
                case ContactSyncReceiverType.Choose:
                    return Choose();
                case ContactSyncReceiverType.Radial:
                    return Radial();
            }
            return new VRCExpressionParameters.Parameter[0];
        }

        IList<VRCExpressionParameters.Parameter> Trigger()
        {
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = ParameterName.ON,
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = true,
                    saved = false,
                },
            };
        }

        IList<VRCExpressionParameters.Parameter> Toggle()
        {
            if (Component.AllowUnconstrained)
            {
                var defaultValue = Component.DefaultValue.NullableBoolValue();
                return new[]
                {
                    new VRCExpressionParameters.Parameter
                    {
                        name = ParameterName.ON,
                        valueType = VRCExpressionParameters.ValueType.Bool,
                        defaultValue = (defaultValue == 1).ToFloat(),
                        networkSynced = true,
                        saved = Component.Saved,
                    },
                    new VRCExpressionParameters.Parameter
                    {
                        name = ParameterName.OFF,
                        valueType = VRCExpressionParameters.ValueType.Bool,
                        defaultValue = (defaultValue == 0).ToFloat(),
                        networkSynced = true,
                        saved = Component.Saved,
                    },
                };
            }
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = ParameterName.ON,
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = Component.DefaultValue.ToBool().ToFloat(),
                    networkSynced = true,
                    saved = Component.Saved,
                },
            };
        }

        IList<VRCExpressionParameters.Parameter> Choose()
        {
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = ParameterName.Value,
                    valueType = VRCExpressionParameters.ValueType.Int,
                    defaultValue = Component.AllowUnconstrained ? Component.DefaultValue + 1 : Component.DefaultValue,
                    networkSynced = true,
                    saved = Component.Saved,
                },
            };
        }

        IList<VRCExpressionParameters.Parameter> Radial()
        {
            if (Component.AllowUnconstrained)
            {
                return new[]
                {
                    new VRCExpressionParameters.Parameter
                    {
                        name = ParameterName.ON,
                        valueType = VRCExpressionParameters.ValueType.Bool,
                        defaultValue = (Component.DefaultValue >= 0).ToFloat(),
                        networkSynced = true,
                        saved = Component.Saved,
                    },
                    new VRCExpressionParameters.Parameter
                    {
                        name = ParameterName.Value,
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = Component.DefaultValue < 0 ? 0 : Component.DefaultValue,
                        networkSynced = true,
                        saved = Component.Saved,
                    },
                };
            }
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = ParameterName.Value,
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = Component.DefaultValue,
                    networkSynced = true,
                    saved = Component.Saved,
                },
            };
        }
    }
}
