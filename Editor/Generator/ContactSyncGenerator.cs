using nadena.dev.modular_avatar.core;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using Narazaka.VRChat.ContactSync.Editor.NameProvider;
using nadena.dev.ndmf;
using VRC.SDK3.Dynamics.Constraint.Components;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class ContactSyncGenerator : IGenerator
    {
        internal readonly BuildContext Context;
        internal readonly ContactSyncAssign Assign;
        internal readonly ContactSyncAssignNameProvider NameProvider;
        internal readonly ContactSyncCommunicator[] Communicators;
        internal AnimatorController Controller;
        internal List<ParameterConfig> Parameters;

        internal GameObject Root;
        internal GameObject Container;

        internal static Vector3 ContainerBaseOffset = new Vector3(0, -200, 0);

        public ContactSyncGenerator(BuildContext ctx, Transform parent, ContactSyncAssign assign)
        {
            Context = ctx;
            Assign = assign;
            NameProvider = new ContactSyncAssignNameProvider(assign);
            Communicators = assign.ContactSyncCommunicators;
            Controller = new AnimatorController();
            var parameters = assign.gameObject.AddComponent<ModularAvatarParameters>();
            Parameters = parameters.parameters = new List<ParameterConfig>();
            Root = GenerateRoot(parent);
            Container = GenerateContainer();

            var ma = Root.AddComponent<ModularAvatarMergeAnimator>();
            ma.animator = Controller;
            ma.layerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX;
            ma.matchAvatarWriteDefaults = true;
            ma.pathMode = MergeAnimatorPathMode.Relative;
        }

        GameObject GenerateRoot(Transform avatarRootTransform)
        {
            var root = avatarRootTransform.CreateGameObjectZero(NameProvider.Name(nameof(Root)));
            root.transform.localPosition = ContainerBaseOffset;
            return root;
        }

        GameObject GenerateContainer()
        {
            var obj = Root.CreateGameObjectZero(NameProvider.Name(nameof(Container)));
            return obj;
        }

        internal string Path(Transform to) => Util.RelativePath(Root.transform, to);
        internal string Path(GameObject to) => Path(to.transform);

        public void Generate()
        {
            new MatchKeyGenerator(this).Generate();
            new AllToggleGenerator(this).Generate();
            GenerateCommunicators();
            Object.DestroyImmediate(Assign);
        }

        void GenerateCommunicators()
        {
            foreach (var item in Communicators)
            {
                GenerateCommunicator(item);
            }
        }

        void GenerateCommunicator(ContactSyncCommunicator com)
        {
            if (com is ContactSyncSender sender)
            {
                new SenderGenerator(this, sender).Generate();
            }
            else if (com is ContactSyncReceiver receiver)
            {
                new ReceiverGenerator(this, receiver).Generate();
            }
        }
    }
}
