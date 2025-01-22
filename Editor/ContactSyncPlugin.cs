using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(Narazaka.VRChat.ContactSync.Editor.ContactSyncPlugin))]

namespace Narazaka.VRChat.ContactSync.Editor
{
    public class ContactSyncPlugin : Plugin<ContactSyncPlugin>
    {
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

                for (var i = 0; i < assigns.Length; ++i)
                {
                    var assign = assigns[i];
                    // var rootName = uniqueNames[i];
                    new Generator.ContactSyncGenerator(ctx, ctx.AvatarRootTransform, assign).Generate();
                }
            });
        }
    }
}
