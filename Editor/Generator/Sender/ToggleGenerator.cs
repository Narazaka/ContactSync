using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync.Editor.Generator.Sender
{
    class ToggleGenerator : SubSenderGenerator
    {
        public ToggleGenerator(ContactSyncGenerator gen, ContactSyncSender com) : base(gen, com)
        {
        }

        protected override void GenerateMenus()
        {
            Component.gameObject.AddComponent<ModularAvatarMenuGroup>();
            var parent = Component.gameObject;
            if (Component.AllowUnconstrained)
            {
                if (Component.HasParentMenu)
                {
                    parent = Component.gameObject.CreateParentMenu(Component.ParentMenu, DefaultMenuName.ParentMenu);
                }
                parent.CreateMenu(Component.OnMenu, VRCExpressionsMenu.Control.ControlType.Toggle, ParameterName.ON, true, DefaultMenuName.OnMenu);
                parent.CreateMenu(Component.OffMenu, VRCExpressionsMenu.Control.ControlType.Toggle, ParameterName.OFF, true, DefaultMenuName.OffMenu);
            }
            else
            {
                parent.CreateMenu(Component.Menu, VRCExpressionsMenu.Control.ControlType.Toggle, ParameterName.ON, true, DefaultMenuName.Menu);
            }
        }

        protected override void GenerateAnimator()
        {
            if (Component.AllowUnconstrained)
            {
                GenerateUnconstrainedAnimator();
            }
            else
            {
                GenerateConstrainedAnimator();
            }
        }

        void GenerateUnconstrainedAnimator()
        {
            var layer = Controller.AddNewLayer(Name(nameof(Contact)));
            layer.EntryPosition(-300, 200);
            var idleState = layer.AddNewState("Idle").Position(0, 200).CreateClip(Name("Idle"), clip => clip.Active(ContactPath, false));
            var offState = layer.AddNewState("OFF").Position(300, 100).CreateClip(Name("OFF"), clip => clip.Active(ContactPath, true).Position(ContactPath, Constants.Toggle.OFF.CenterPosition)).AddParameterDriver(SetFalse(ParameterName.ON));
            var onState = layer.AddNewState("ON").Position(300, 300).CreateClip(Name("ON"), clip => clip.Active(ContactPath, true).Position(ContactPath, Constants.Toggle.ON.CenterPosition)).AddParameterDriver(SetFalse(ParameterName.OFF));
            layer.DefaultState(idleState);
            idleState.AddTransition(offState).If(ParameterName.OFF);
            idleState.AddTransition(onState).If(ParameterName.ON);
            offState.AddTransition(onState).If(ParameterName.ON);
            offState.AddTransition(idleState).IfNot(ParameterName.OFF);
            onState.AddTransition(offState).If(ParameterName.OFF);
            onState.AddTransition(idleState).IfNot(ParameterName.ON);
        }

        VRC_AvatarParameterDriver.Parameter SetFalse(string parameter) => new VRC_AvatarParameterDriver.Parameter { type = VRC_AvatarParameterDriver.ChangeType.Set, value = 0, name = parameter };

        void GenerateConstrainedAnimator()
        {
            var layer = Controller.AddNewLayer(Name(nameof(Contact)));
            layer.EntryPosition(0, 0);
            var offState = layer.AddNewState("OFF").Position(0, 100).CreateClip(Name("OFF"), clip => clip.Active(ContactPath, true).Position(ContactPath, Constants.Toggle.OFF.CenterPosition));
            var onState = layer.AddNewState("ON").Position(0, 200).CreateClip(Name("ON"), clip => clip.Active(ContactPath, true).Position(ContactPath, Constants.Toggle.ON.CenterPosition));
            layer.ExitPosition(0, 300);
            layer.DefaultState(offState);
            offState.AddTransition(onState).If(ParameterName.ON);
            onState.AddExitTransition().IfNot(ParameterName.ON);
        }
    }
}
