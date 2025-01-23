using nadena.dev.modular_avatar.core;
using Narazaka.VRChat.ContactSync.Editor.NameProvider;
using Narazaka.VRChat.ContactSync.Editor.ParameterProvider;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class MatchKeyGenerator : SubGenerator
    {
        readonly ContactSyncMatchKey MatchKey;
        readonly ContactSyncMatchKeyNameProvider NameProvider;
        readonly ContactSyncMatchKeyParameterProvider ParameterProvider;
        readonly ContactSyncMatchKeyNameProvider.ParameterNames ParameterName;

        const float MatchKeyPositionScale = 4;
        static int HalfValue = Mathf.CeilToInt(ContactSyncMatchKey.MaxValue / 2f);
        static Vector3 OffsetPosition = new Vector3(-HalfValue, 0, -HalfValue) * MatchKeyPositionScale;

        GameObject MatchKeyOffset;
        GameObject MatchKeyA;
        GameObject MatchKeyB;

        public MatchKeyGenerator(ContactSyncGenerator gen) : base(gen)
        {
            MatchKey = ContactSyncGenerator.Assign.ContactSyncMatchKey;
            if (MatchKey == null) return;

            NameProvider = new ContactSyncMatchKeyNameProvider(MatchKey);
            ParameterProvider = new ContactSyncMatchKeyParameterProvider(MatchKey);
            ParameterName = NameProvider.ParameterName;
        }

        protected string Name(string name) => NameProvider.Name(name);

        public override void Generate()
        {
            if (MatchKey == null) return;

            MatchKeyOffset = ContactSyncGenerator.Root.CreateGameObjectZero(nameof(MatchKeyOffset));
            MatchKeyOffset.transform.localPosition = OffsetPosition;
            MatchKeyA = MatchKeyOffset.CreateGameObjectZero(nameof(MatchKeyA));
            MatchKeyA.transform.localPosition = new Vector3(MatchKey.MatchKeyA * MatchKeyPositionScale, 0, 0);
            MatchKeyB = MatchKeyA.CreateGameObjectZero(nameof(MatchKeyB));
            MatchKeyB.transform.localPosition = new Vector3(0, 0, MatchKey.MatchKeyB * MatchKeyPositionScale);
            Container.SetParentZero(MatchKeyB);

            GenerateParameters();
            GenerateMenus();
            GenerateAnimator();
            Object.DestroyImmediate(MatchKey);
        }

        void GenerateParameters()
        {
            var controllerParameters = ParameterProvider.GetParameters();
            AddParameters(controllerParameters);
            AddParametersToController(controllerParameters);
        }

        void GenerateMenus()
        {
            var menuGroup = MatchKey.gameObject.AddComponent<ModularAvatarMenuGroup>();
            var parentGameObject = MatchKey.gameObject;
            if (MatchKey.HasParentMenu)
            {
                parentGameObject = MatchKey.gameObject.CreateParentMenu(MatchKey.ParentMenu, ContactSyncMatchKey.DefaultMenuName.Parent);
            }
            parentGameObject.CreateRadialMenu(MatchKey.MatchKeyAMenu, ParameterName.MatchKeyAUI, ContactSyncMatchKey.DefaultMenuName.MatchKeyA);
            parentGameObject.CreateRadialMenu(MatchKey.MatchKeyBMenu, ParameterName.MatchKeyBUI, ContactSyncMatchKey.DefaultMenuName.MatchKeyB);
            parentGameObject.CreateMenu(MatchKey.MatchKeySyncMenu, VRCExpressionsMenu.Control.ControlType.Button, ParameterName.Sync, true, ContactSyncMatchKey.DefaultMenuName.MatchKeySync);
            if (MatchKey.HasRandomizeMenu)
            {
                parentGameObject.CreateMenu(MatchKey.MatchKeyRandomizeMenu, VRCExpressionsMenu.Control.ControlType.Button, ParameterName.Randomize, true, ContactSyncMatchKey.DefaultMenuName.MatchKeyRandomize);
            }
        }

        void GenerateAnimator()
        {
            GeneratePositionAnimator();
            GenerateSyncAnimator();
            if (MatchKey.HasRandomizeMenu) GenerateRandomizeAnimator();
        }

        void GeneratePositionAnimator()
        {
            var layerX = Controller.AddNewLayer(Name("PositionX"));
            layerX.EntryPosition(0, 0);
            layerX.AddNewState(nameof(ParameterName.MatchKeyA)).Position(0, 100).CreateClip(Name("PositionX"), clip => clip.PositionFromTo(Path(MatchKeyA), Vector3.zero, new Vector3(ContactSyncMatchKey.MaxValue * MatchKeyPositionScale, 0, 0))).TimeParameter(ParameterName.MatchKeyA);
            var layerZ = Controller.AddNewLayer(Name("PositionZ"));
            layerZ.EntryPosition(0, 0);
            layerZ.AddNewState(nameof(ParameterName.MatchKeyB)).Position(0, 100).CreateClip(Name("PositionZ"), clip => clip.PositionFromTo(Path(MatchKeyB), Vector3.zero, new Vector3(0, 0, ContactSyncMatchKey.MaxValue * MatchKeyPositionScale))).TimeParameter(ParameterName.MatchKeyB);
        }

        void GenerateSyncAnimator()
        {
            var layer = Controller.AddNewLayer(Name("Sync"));
            layer.EntryPosition(0, 0);
            var initState = layer.AddNewState("Init").Position(0, 100)
                .AddParameterDriver(CopyToUI(ParameterName.MatchKeyA, ParameterName.MatchKeyAUI))
                .AddParameterDriver(CopyToUI(ParameterName.MatchKeyB, ParameterName.MatchKeyBUI));
            var syncState = layer.AddNewState("Sync").Position(0, 200)
                .AddParameterDriver(CopyFromUI(ParameterName.MatchKeyAUI, ParameterName.MatchKeyA))
                .AddParameterDriver(CopyFromUI(ParameterName.MatchKeyBUI, ParameterName.MatchKeyB));
            layer.ExitPosition(0, 300);
            layer.DefaultState(initState);
            initState.AddTransition(syncState).If(ParameterName.Sync);
            syncState.AddExitTransition().IfNot(ParameterName.Sync);
        }

        void GenerateRandomizeAnimator()
        {
            var layer = Controller.AddNewLayer(Name("Randomize"));
            layer.EntryPosition(0, 0);
            var initState = layer.AddNewState("Init").Position(0, 100);
            var randomizeState = layer.AddNewState("Randomize").Position(0, 200)
                .AddParameterDriver(Randomize(ParameterName.MatchKeyAUI))
                .AddParameterDriver(Randomize(ParameterName.MatchKeyBUI))
                .AddParameterDriver(CopyFromUI(ParameterName.MatchKeyAUI, ParameterName.MatchKeyA))
                .AddParameterDriver(CopyFromUI(ParameterName.MatchKeyBUI, ParameterName.MatchKeyB));
            layer.ExitPosition(0, 300);
            initState.AddTransition(randomizeState).If(ParameterName.Randomize);
            randomizeState.AddExitTransition().IfNot(ParameterName.Randomize);
        }

        VRC_AvatarParameterDriver.Parameter CopyToUI(string sync, string ui) => new VRC_AvatarParameterDriver.Parameter
        {
            type = VRC_AvatarParameterDriver.ChangeType.Copy,
            source = sync,
            name = ui,
        };

        VRC_AvatarParameterDriver.Parameter CopyFromUI(string ui, string sync) => new VRC_AvatarParameterDriver.Parameter
        {
            type = VRC_AvatarParameterDriver.ChangeType.Copy,
            source = ui,
            name = sync,
        };

        VRC_AvatarParameterDriver.Parameter Randomize(string ui) => new VRC_AvatarParameterDriver.Parameter
        {
            type = VRC_AvatarParameterDriver.ChangeType.Random,
            name = ui,
            valueMin = 0,
            valueMax = 1,
        };
    }
}
