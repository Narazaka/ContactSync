using nadena.dev.modular_avatar.core;
using Narazaka.VRChat.ContactSync.Editor.NameProvider;
using Narazaka.VRChat.ContactSync.Editor.ParameterProvider;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class AllToggleGenerator : SubGenerator
    {
        readonly ContactSyncAllToggle AllToggle;
        readonly ContactSyncAllToggleNameProvider NameProvider;
        readonly ContactSyncAllToggleParameterProvider ParameterProvider;
        readonly string ParameterName;

        public AllToggleGenerator(ContactSyncGenerator gen) : base(gen)
        {
            AllToggle = ContactSyncGenerator.Assign.ContactSyncAllToggle;
            if (AllToggle == null) return;
            NameProvider = new ContactSyncAllToggleNameProvider(AllToggle);
            ParameterProvider = new ContactSyncAllToggleParameterProvider(AllToggle);
            ParameterName = NameProvider.ParameterName;
        }

        public override void Generate()
        {
            if (AllToggle == null) return;

            Container.SetActive(AllToggle.Enabled);

            GenerateParameters();
            GenerateMenus();
            GenerateAnimator();
            Object.DestroyImmediate(AllToggle);
        }

        void GenerateParameters()
        {
            var parameters = ParameterProvider.GetParameters();
            AddParameters(parameters);
            AddParametersToController(parameters);
        }

        void GenerateMenus()
        {
            AllToggle.gameObject.AddComponent<ModularAvatarMenuGroup>();
            AllToggle.gameObject.CreateMenu(AllToggle.Menu, VRCExpressionsMenu.Control.ControlType.Toggle, ParameterName, true, ContactSyncAllToggle.DefaultMenuName.Menu);
        }

        void GenerateAnimator()
        {
            var layer = Controller.AddNewLayer(NameProvider.Name("Base"));
            layer.EntryPosition(0, 0);
            var offState = layer.AddNewState("OFF").Position(0, 100).CreateClip(NameProvider.Name("OFF"), clip => clip.Active(Path(Container), false));
            var onState = layer.AddNewState("ON").Position(0, 200).CreateClip(NameProvider.Name("ON"), clip => clip.Active(Path(Container), true));
            layer.ExitPosition(0, 300);
            layer.DefaultState(offState);
            offState.AddTransition(onState).If(ParameterName);
            onState.AddExitTransition().IfNot(ParameterName);
        }
    }
}
