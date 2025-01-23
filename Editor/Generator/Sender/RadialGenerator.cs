using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.Generator.Sender
{
    class RadialGenerator : SubSenderGenerator
    {
        public RadialGenerator(ContactSyncGenerator gen, ContactSyncSender com) : base(gen, com)
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
                parent.CreateRadialMenu(Component.RadialMenu, ParameterName.Value, DefaultMenuName.RadialMenu);
            }
            else
            {
                parent.CreateRadialMenu(Component.RadialMenu, ParameterName.Value, DefaultMenuName.RadialMenu);
            }
        }

        protected override void GenerateAnimator()
        {
            var layer = Controller.AddNewLayer(Name(nameof(Contact)));
            layer.EntryPosition(0, 0);

            var state = layer.AddNewState("Value").Position(0, 200).CreateClip(Name("Value"), clip => clip.Active(ContactPath, true).PositionFromTo(ContactPath, Constants.Radial.Min.CenterPosition, Constants.Radial.Max.CenterPosition))
                .TimeParameter(ParameterName.Value);

            if (Component.AllowUnconstrained)
            {
                var idleState = layer.AddNewState("Idle").Position(0, 100).CreateClip(Name("Idle"), clip => clip.Active(ContactPath, false));
                layer.ExitPosition(0, 300);
                layer.DefaultState(idleState);
                idleState.AddTransition(state).If(ParameterName.ON);
                state.AddExitTransition().IfNot(ParameterName.ON);
            }
        }
    }
}
