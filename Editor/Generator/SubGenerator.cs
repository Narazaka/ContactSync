using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using Narazaka.VRChat.AvatarParametersUtil;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    abstract class SubGenerator : IGenerator
    {
        protected readonly ContactSyncGenerator ContactSyncGenerator;

        public SubGenerator(ContactSyncGenerator gen)
        {
            ContactSyncGenerator = gen;
        }

        protected GameObject Container => ContactSyncGenerator.Container;
        protected string Path(Transform to) => ContactSyncGenerator.Path(to);
        protected string Path(GameObject to) => ContactSyncGenerator.Path(to);
        protected List<ParameterConfig> Parameters => ContactSyncGenerator.Parameters;
        protected AnimatorController Controller => ContactSyncGenerator.Controller;
        protected AnimatorController DirectController => ContactSyncGenerator.DirectController;

        public abstract void Generate();

        protected void AddParametersToController(IEnumerable<VRCExpressionParameters.Parameter> parameters)
        {
            foreach (var p in parameters)
            {
                Controller.AddParameter(p.ToAnimatorControllerParameter());
            }
        }

        protected void AddParametersToDirectController(IEnumerable<VRCExpressionParameters.Parameter> parameters)
        {
            foreach (var p in parameters)
            {
                DirectController.AddParameter(p.ToAnimatorControllerParameter());
            }
        }

        protected void AddParameters(IEnumerable<VRCExpressionParameters.Parameter> parameters)
        {
            foreach (var p in parameters)
            {
                Parameters.Add(ToParameterConfig(p));
            }
        }

        ParameterConfig ToParameterConfig(VRCExpressionParameters.Parameter parameter)
        {
            return new ParameterConfig
            {
                nameOrPrefix = parameter.name,
                syncType = GetParameterSyncType(parameter.valueType),
                hasExplicitDefaultValue = true,
                defaultValue = parameter.defaultValue,
                saved = parameter.saved,
                localOnly = !parameter.networkSynced,
            };
        }

        ParameterSyncType GetParameterSyncType(VRCExpressionParameters.ValueType valueType)
        {
            switch (valueType)
            {
                case VRCExpressionParameters.ValueType.Bool:
                    return ParameterSyncType.Bool;
                case VRCExpressionParameters.ValueType.Float:
                    return ParameterSyncType.Float;
                case VRCExpressionParameters.ValueType.Int:
                    return ParameterSyncType.Int;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}
