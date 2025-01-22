using nadena.dev.ndmf;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using Narazaka.VRChat.AvatarParametersUtil;

namespace Narazaka.VRChat.ContactSync.Editor.ParameterProvider
{
    abstract class BaseParameterProvider<C, N> : IParameterProvider where C : Component where N : NameProvider.BaseNameProvider<C>
    {
        protected readonly C Component;
        protected readonly N NameProvider;

        public BaseParameterProvider(C component)
        {
            Component = component;
            NameProvider = (N)System.Activator.CreateInstance(typeof(N), new object[] { component });
        }

        public IEnumerable<ProvidedParameter> GetSuppliedParameters(BuildContext context = null)
        {
            if (!NameProvider.Valid) return new ProvidedParameter[0];
            return GetParameters().Select(p => new ProvidedParameter(
                p.name,
                ParameterNamespace.Animator,
                Component,
                ContactSyncPlugin.Instance,
                p.valueType.ToAnimatorControllerParameterType()
                )
            {
                DefaultValue = p.defaultValue,
                WantSynced = p.networkSynced,
            });
        }

        public abstract IList<VRCExpressionParameters.Parameter> GetParameters();
    }
}
