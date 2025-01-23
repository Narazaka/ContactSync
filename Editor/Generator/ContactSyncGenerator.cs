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
        internal AnimatorController DirectController;
        internal List<ParameterConfig> Parameters;

        internal GameObject Root;
        internal GameObject Container;

        internal static Vector3 ContainerBaseOffset = new Vector3(0, -200, 0);

        static Transform _fixPrefab;
        static Transform FixPrefab => _fixPrefab ??= AssetDatabase.LoadAssetAtPath<GameObject>("Packages/net.narazaka.vrchat.contact-sync/Assets/Origin.prefab").transform;

        public ContactSyncGenerator(BuildContext ctx, Transform avatarRootTransform, ContactSyncAssign assign)
        {
            Context = ctx;
            Assign = assign;
            NameProvider = new ContactSyncAssignNameProvider(assign);
            Communicators = assign.ContactSyncCommunicators;
            Controller = new AnimatorController();
            DirectController = new AnimatorController();
            var parameters = assign.gameObject.AddComponent<ModularAvatarParameters>();
            Parameters = parameters.parameters = new List<ParameterConfig>();
            Root = GenerateRoot(avatarRootTransform);
            Container = GenerateContainer();

            var ma = Root.AddComponent<ModularAvatarMergeAnimator>();
            ma.animator = Controller;
            ma.layerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX;
            ma.matchAvatarWriteDefaults = true;
            ma.pathMode = MergeAnimatorPathMode.Relative;

            var directMa = Root.AddComponent<ModularAvatarMergeAnimator>();
            directMa.animator = DirectController;
            directMa.layerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX;
            directMa.matchAvatarWriteDefaults = false;
            directMa.pathMode = MergeAnimatorPathMode.Relative;
        }

        GameObject GenerateRoot(Transform avatarRootTransform)
        {
            var root = avatarRootTransform.CreateGameObjectZero(NameProvider.Name(nameof(Root)));
            var avatarScale = avatarRootTransform.localScale;
            root.transform.localScale = new Vector3(1 / avatarScale.x, 1 / avatarScale.y, 1 / avatarScale.z);
            // root.AddComponent<ModularAvatarWorldFixedObject>();
            var c = root.AddComponent<VRCParentConstraint>();
            c.AffectsPositionX = true;
            c.AffectsPositionY = true;
            c.AffectsPositionZ = true;
            c.AffectsRotationX = true;
            c.AffectsRotationY = true;
            c.AffectsRotationZ = true;
            c.Locked = true;
            c.IsActive = true;
            c.GlobalWeight = 1;
            c.Sources.Add(new VRC.Dynamics.VRCConstraintSource(FixPrefab, 1, Vector3.zero, Vector3.zero));

            return root;
        }

        GameObject GenerateContainer()
        {
            var obj = Root.CreateGameObjectZero(NameProvider.Name(nameof(Container)));
            obj.transform.localPosition = ContainerBaseOffset;
            return obj;
        }

        internal string Path(Transform to) => Util.RelativePath(Root.transform, to);
        internal string Path(GameObject to) => Path(to.transform);

        public void Generate()
        {
            new AllToggleGenerator(this).Generate();
            new MatchKeyGenerator(this).Generate();
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
