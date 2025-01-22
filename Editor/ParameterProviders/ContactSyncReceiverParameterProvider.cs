using nadena.dev.ndmf;
using Narazaka.VRChat.AvatarParametersUtil;
using System.Collections.Generic;
using System.Linq;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.ParameterProvider
{
    [ParameterProviderFor(typeof(ContactSyncReceiver))]
    class ContactSyncReceiverParameterProvider : BaseParameterProvider<ContactSyncReceiver, NameProvider.ContactSyncReceiverNameProvider>
    {
        NameProvider.ContactSyncReceiverNameProvider.ParameterNames ParameterName;
        public ContactSyncReceiverParameterProvider(ContactSyncReceiver component) : base(component)
        {
            ParameterName = NameProvider.ParameterName;
        }

        public override IList<VRCExpressionParameters.Parameter> GetParameters()
        {
            return GetContactParameters().Concat(GetMenuParameters()).ToArray();
        }

        public IList<VRCExpressionParameters.Parameter> GetContactParameters()
        {
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = ParameterName.Contact,
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = 0,
                    networkSynced = !Component.LocalOnly,
                },
            };
        }

        public IList<VRCExpressionParameters.Parameter> GetMenuParameters()
        {
            if (!Component.UseDisableMenu) return new VRCExpressionParameters.Parameter[0];
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = ParameterName.Enabled,
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = Component.Enabled.ToFloat(),
                    networkSynced = !Component.LocalOnly,
                    saved = Component.Saved,
                },
            };
        }

        // do not use on GetParameters because external
        public IList<VRCExpressionParameters.Parameter> GetExternalParameters(BuildContext context = null)
        {
            var info = context == null ? ParameterInfo.ForUI : ParameterInfo.ForContext(context);
            var avatar = Component.GetComponentInParent<VRCAvatarDescriptor>();
            if (avatar == null) return new VRCExpressionParameters.Parameter[0];

            var parameters = info.GetParametersForObject(avatar.gameObject).ToDistinctSubParameters().NotEmpty().ToArray();
            var parameter = parameters.FirstOrDefault(p => p.EffectiveName == Component.ParameterName);
            var valueType = parameter?.ParameterType == null ? TagParameterType() : parameter.ParameterType.Value.ToVRCExpressionParametersValueType();
            if (valueType == null) return new VRCExpressionParameters.Parameter[0];
            return new[]
            {
                new VRCExpressionParameters.Parameter
                {
                    name = Component.ParameterName,
                    valueType = valueType.Value,
                    defaultValue = parameter?.DefaultValue ?? 0,
                    networkSynced = parameter?.WantSynced ?? false,
                },
            };
        }

        VRCExpressionParameters.ValueType? TagParameterType()
        {
            var assign = Component.GetComponentInParent<ContactSyncAssign>();
            if (assign == null || assign.ContactSyncTagGroup == null) return null;
            var tag = assign.ContactSyncTagGroup.Tags.FirstOrDefault(t => t.Name == Component.TagName);
            if (tag == null) return null;
            switch (tag.ReceiverType)
            {
                case ContactSyncReceiverType.Trigger:
                case ContactSyncReceiverType.Toggle:
                    return VRCExpressionParameters.ValueType.Bool;
                case ContactSyncReceiverType.Choose:
                    return VRCExpressionParameters.ValueType.Int;
                case ContactSyncReceiverType.Radial:
                    return VRCExpressionParameters.ValueType.Float;
                default:
                    return null;
            }
        }
    }
}
