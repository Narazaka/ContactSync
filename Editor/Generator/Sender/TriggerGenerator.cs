using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync.Editor.Generator.Sender
{
    class TriggerGenerator : SubSenderGenerator
    {
        public TriggerGenerator(ContactSyncGenerator gen, ContactSyncSender com) : base(gen, com)
        {
        }

        protected override void GenerateMenus()
        {
            Component.gameObject.AddComponent<ModularAvatarMenuGroup>();
            var parent = Component.gameObject;
            parent.CreateMenu(Component.Menu, Component.UseToggleButton ? VRCExpressionsMenu.Control.ControlType.Toggle : VRCExpressionsMenu.Control.ControlType.Button, ParameterName.ON, true, DefaultMenuName.Menu);
        }

        protected override void GenerateAnimator()
        {
            var layer = Controller.AddNewLayer(Name(nameof(Contact)));
            layer.EntryPosition(0, 0);
            var offState = layer.AddNewState("OFF").Position(0, 100).CreateClip(Name("OFF"), clip => clip.Active(ContactPath, false));
            var onState = layer.AddNewState("ON").Position(0, 200).CreateClip(Name("ON"), clip => clip.Active(ContactPath, true));
            layer.ExitPosition(0, 300);
            layer.DefaultState(offState);
            offState.AddTransition(onState).If(ParameterName.ON);
            onState.AddExitTransition().IfNot(ParameterName.ON);
        }
    }
}
