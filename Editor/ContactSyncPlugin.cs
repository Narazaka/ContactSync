using nadena.dev.ndmf;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.Constraint.Components;

[assembly: ExportsPlugin(typeof(Narazaka.VRChat.ContactSync.Editor.ContactSyncPlugin))]

namespace Narazaka.VRChat.ContactSync.Editor
{
    public class ContactSyncPlugin : Plugin<ContactSyncPlugin>
    {
        public override string DisplayName => "Contact Sync";
        public override string QualifiedName => "net.narazaka.vrchat.contact-sync";
        protected override void Configure()
        {
            InPhase(BuildPhase.Generating).BeforePlugin("nadena.dev.modular-avatar").Run("ContactSync", (ctx) =>
            {
                var assigns = ctx.AvatarRootTransform.GetComponentsInChildren<ContactSyncAssign>();

                /*
                var uniqueNames = new List<string>(assigns.Length);
                foreach (var group in assigns)
                {
                    var name = group.name;
                    while (uniqueNames.Contains(name))
                    {
                        name += "_";
                    }
                    uniqueNames.Add(group.name);
                }
                */

                var root = CreateFixedRoot(ctx.AvatarRootTransform);
                for (var i = 0; i < assigns.Length; ++i)
                {
                    var assign = assigns[i];
                    // var rootName = uniqueNames[i];
                    new Generator.ContactSyncGenerator(ctx, root.transform, assign).Generate();
                }
            });
        }

        GameObject CreateFixedRoot(Transform avatarRootTransform)
        {
            var fixPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/net.narazaka.vrchat.contact-sync/Assets/Origin.prefab").transform;

            var root = avatarRootTransform.CreateGameObjectZero("ContactSync");
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
            c.Sources.Add(new VRC.Dynamics.VRCConstraintSource(fixPrefab, 1, Vector3.zero, Vector3.zero));

            var avatarScale = avatarRootTransform.localScale;
            root.transform.localScale = new Vector3(1 / avatarScale.x, 1 / avatarScale.y, 1 / avatarScale.z);

            return root;
        }
    }
}
