using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor.Generator.Sender
{
    class ChooseGenerator : SubSenderGenerator
    {
        public ChooseGenerator(ContactSyncGenerator gen, ContactSyncSender com) : base(gen, com)
        {
        }

        protected override void GenerateMenus()
        {
            Component.gameObject.AddComponent<ModularAvatarMenuGroup>();
            var parent = Component.gameObject;
            if (Component.HasParentMenu)
            {
                parent = Component.gameObject.CreateParentMenu(Component.ParentMenu, DefaultMenuName.ParentMenu);
            }
            for (var i = 0; i < Component.ChooseMenus.Length; i++)
            {
                var value = Component.AllowUnconstrained ? i + 1 : i;
                parent.CreateMenu(Component.ChooseMenus[i], VRCExpressionsMenu.Control.ControlType.Toggle, ParameterName.Value, value, DefaultMenuName.ChooseMenu(i));
            }
        }

        protected override void GenerateAnimator()
        {
            var layer = Controller.AddNewLayer(Name(nameof(Contact)));
            layer.EntryPosition(-300, 0);
            layer.ExitPosition(300, 0);

            if (Component.AllowUnconstrained)
            {
                var idleState = layer.AddNewState("Idle").Position(0, -100).CreateClip(Name("Idle"), clip => clip.Active(ContactPath, true).Position(ContactPath, UnconstrainedPosition));
                layer.AddEntryTransition(idleState).Equal(ParameterName.Value, 0);
                idleState.AddExitTransition().NotEqual(ParameterName.Value, 0);
                layer.DefaultState(idleState);
            }
            for (var i = 0; i < Component.ChooseMenus.Length; i++)
            {
                var value = Component.AllowUnconstrained ? i + 1 : i;
                var state = layer.AddNewState($"Choice {i}").Position(0, i * 100).CreateClip(Name($"Choice {i}"), clip => clip.Active(ContactPath, true).Position(ContactPath, ChooseConstant.MinPosition + ChooseConstant.Step * i));
                layer.AddEntryTransition(state).Equal(ParameterName.Value, value);
                state.AddExitTransition().NotEqual(ParameterName.Value, value);
                if (!Component.AllowUnconstrained && i == 0)
                {
                    layer.DefaultState(state);
                }
            }
        }
    }
}
