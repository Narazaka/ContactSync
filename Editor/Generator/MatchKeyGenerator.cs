using nadena.dev.modular_avatar.core;
using Narazaka.VRChat.ContactSync.Editor.NameProvider;
using Narazaka.VRChat.ContactSync.Editor.ParameterProvider;
using System.Linq;
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

        const float MatchKeyPositionScale = 3;
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
            var coreParameters = ParameterProvider.GetCoreParameters();
            AddParameters(coreParameters);
            foreach (var p in coreParameters)
            {
                // int to float because 1D Blend Tree only supports float
                p.valueType = VRCExpressionParameters.ValueType.Float;
            }
            AddParametersToController(coreParameters);
            var uiParameters = ParameterProvider.GetUIParameters();
            AddParameters(uiParameters);
            AddParametersToController(uiParameters);
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
            layerX.AddNewState(nameof(ParameterName.MatchKeyA)).Position(0, 100)
                .Create1DBlendTree(Name("PositionX"), ParameterName.MatchKeyA, tree => tree
                    .Add1DChildClip(Name("PositionXMin"), 0, clip => clip.Position(Path(MatchKeyA), Vector3.zero))
                    .Add1DChildClip(Name("PositionXMax"), ContactSyncMatchKey.MaxValue, clip => clip.Position(Path(MatchKeyA), new Vector3(ContactSyncMatchKey.MaxValue * MatchKeyPositionScale, 0, 0)))
                    );
            var layerZ = Controller.AddNewLayer(Name("PositionZ"));
            layerZ.EntryPosition(0, 0);
            layerZ.AddNewState(nameof(ParameterName.MatchKeyB)).Position(0, 100)
                .Create1DBlendTree(Name("PositionZ"), ParameterName.MatchKeyB, tree => tree
                    .Add1DChildClip(Name("PositionZMin"), 0, clip => clip.Position(Path(MatchKeyB), Vector3.zero))
                    .Add1DChildClip(Name("PositionZMax"), ContactSyncMatchKey.MaxValue, clip => clip.Position(Path(MatchKeyB), new Vector3(0, 0, ContactSyncMatchKey.MaxValue * MatchKeyPositionScale))));
        }

        void GenerateSyncAnimator()
        {
            var layer = Controller.AddNewLayer(Name("Sync"));
            layer.EntryPosition(0, 0);
            var initState = layer.AddNewState("Init").Position(0, 100)
                .AddParameterDriver(CopySyncToUI(ParameterName.MatchKeyA, ParameterName.MatchKeyAUI))
                .AddParameterDriver(CopySyncToUI(ParameterName.MatchKeyB, ParameterName.MatchKeyBUI));
            var syncState = layer.AddNewState("Sync").Position(0, 200)
                .AddParameterDriver(CopyUIToFloat(ParameterName.MatchKeyAUI, ParameterName.MatchKeyAFloat))
                .AddParameterDriver(CopyUIToFloat(ParameterName.MatchKeyBUI, ParameterName.MatchKeyBFloat))
                .AddParameterDriver(CopyFloatToInt(ParameterName.MatchKeyAFloat, ParameterName.MatchKeyAInt))
                .AddParameterDriver(CopyFloatToInt(ParameterName.MatchKeyBFloat, ParameterName.MatchKeyBInt))
                .AddParameterDriver(CopyIntToSync(ParameterName.MatchKeyAInt, ParameterName.MatchKeyA))
                .AddParameterDriver(CopyIntToSync(ParameterName.MatchKeyBInt, ParameterName.MatchKeyB));
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
                .AddParameterDriver(RandomizeInt(ParameterName.MatchKeyAInt))
                .AddParameterDriver(RandomizeInt(ParameterName.MatchKeyBInt))
                .AddParameterDriver(CopyIntToSync(ParameterName.MatchKeyAInt, ParameterName.MatchKeyA))
                .AddParameterDriver(CopyIntToSync(ParameterName.MatchKeyBInt, ParameterName.MatchKeyB))
                .AddParameterDriver(CopySyncToUI(ParameterName.MatchKeyA, ParameterName.MatchKeyAUI))
                .AddParameterDriver(CopySyncToUI(ParameterName.MatchKeyB, ParameterName.MatchKeyBUI));
            layer.ExitPosition(0, 300);
            initState.AddTransition(randomizeState).If(ParameterName.Randomize);
            randomizeState.AddExitTransition().IfNot(ParameterName.Randomize);
        }

        VRC_AvatarParameterDriver.Parameter CopySyncToUI(string sync, string ui) => new VRC_AvatarParameterDriver.Parameter
        {
            type = VRC_AvatarParameterDriver.ChangeType.Copy,
            source = sync,
            name = ui,
            convertRange = true,
            sourceMin = 0,
            sourceMax = ContactSyncMatchKey.MaxValue,
            destMin = 0,
            destMax = 1,
        };

        // for rounding
        VRC_AvatarParameterDriver.Parameter CopyUIToFloat(string ui, string f) => new VRC_AvatarParameterDriver.Parameter
        {
            type = VRC_AvatarParameterDriver.ChangeType.Copy,
            source = ui,
            name = f,
            convertRange = true,
            sourceMin = 0,
            sourceMax = 1,
            destMin = 0.5f,
            destMax = ContactSyncMatchKey.MaxValue + 0.5f,
        };

        // cast float to int for local use
        // because sync is float param
        VRC_AvatarParameterDriver.Parameter CopyFloatToInt(string f, string i) => new VRC_AvatarParameterDriver.Parameter
        {
            type = VRC_AvatarParameterDriver.ChangeType.Copy,
            source = f,
            name = i,
        };

        VRC_AvatarParameterDriver.Parameter CopyIntToSync(string i, string sync) => new VRC_AvatarParameterDriver.Parameter
        {
            type = VRC_AvatarParameterDriver.ChangeType.Copy,
            source = i,
            name = sync,
        };

        VRC_AvatarParameterDriver.Parameter RandomizeInt(string i) => new VRC_AvatarParameterDriver.Parameter
        {
            type = VRC_AvatarParameterDriver.ChangeType.Random,
            name = i,
            valueMin = 0,
            valueMax = ContactSyncMatchKey.MaxValue,
        };
    }
}
