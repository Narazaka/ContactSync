using nadena.dev.modular_avatar.core;
using Narazaka.VRChat.ContactSync.Editor.NameProvider;
using Narazaka.VRChat.ContactSync.Editor.ParameterProvider;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Dynamics.Contact.Components;
using VRC.SDKBase;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class SenderGenerator : CommunicatorGenerator<ContactSyncSender>
    {
        public SenderGenerator(ContactSyncGenerator gen, ContactSyncSender com) : base(gen, com)
        {
        }

        public override void Generate()
        {
            switch (Tag.ReceiverType)
            {
                case ContactSyncReceiverType.Toggle:
                    GetSubGenerator<ToggleGenerator>().Generate();
                    break;
            }
        }

        abstract class SubSenderGenerator : CommunicatorSubGenerator<ContactSyncSender>
        {
            protected readonly ContactSyncSenderNameProvider NameProvider;
            protected readonly ContactSyncSenderNameProvider.ParameterNames ParameterName;
            protected readonly ContactSyncSenderParameterProvider ParameterProvider;
            protected readonly ContactSyncSender.DefaultMenuName DefaultMenuName;
            protected GameObject Contact;
            protected string ContactPath;
            public SubSenderGenerator(ContactSyncGenerator gen, ContactSyncSender com) : base(gen, com)
            {
                NameProvider = new ContactSyncSenderNameProvider(com);
                ParameterName = NameProvider.ParameterName;
                ParameterProvider = new ContactSyncSenderParameterProvider(com);
                DefaultMenuName = new ContactSyncSender.DefaultMenuName(com.name);
            }
            protected static Vector3 UnconstrainedPosition = Vector3.down * 1.5f;
            protected string Name(string name) => NameProvider.Name(name);

            public override sealed void Generate()
            {
                GenerateParameters();
                GenerateMenus();
                Contact = GenerateContact();
                ContactPath = Path(Contact);
                GenerateAnimator();
                Object.DestroyImmediate(Component);
            }

            protected void GenerateParameters()
            {
                var parameters = ParameterProvider.GetParameters();
                AddParameters(parameters);
                AddParametersToController(parameters);
            }

            protected abstract void GenerateMenus();
            protected abstract void GenerateAnimator();
            protected virtual GameObject GenerateContact()
            {
                var contact = GenerateBaseContact(Name(nameof(Contact)));
                contact.gameObject.SetActive(false);
                return contact.gameObject;
            }

            protected VRCContactSender GenerateBaseContact(string objectName)
            {
                var objContainer = Container.CreateGameObjectZero($"{objectName}_Container");
                objContainer.transform.localPosition = TagLocalPositionBase;
                var obj = objContainer.CreateGameObjectZero(objectName);
                obj.transform.localPosition = UnconstrainedPosition;
                var contact = obj.AddComponent<VRCContactSender>();
                contact.shapeType = ContactBase.ShapeType.Sphere;
                contact.radius = 0.01f;
                contact.collisionTags = CollisionTags;
                return contact;
            }
        }

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

            static Vector3 OFFPosition = Vector3.down * 0.75f;
            static Vector3 ONPosition = Vector3.zero;

            void GenerateUnconstrainedAnimator()
            {
                var layer = Controller.AddNewLayer(Name(nameof(Contact)));
                layer.EntryPosition(-300, 200);
                var idleState = layer.AddNewState("Idle").Position(0, 200).CreateClip(Name("Idle"), clip => clip.Active(ContactPath, false));
                var offState = layer.AddNewState("OFF").Position(300, 100).CreateClip(Name("OFF"), clip => clip.Active(ContactPath, true).Position(ContactPath, OFFPosition)).AddParameterDriver(SetFalse(ParameterName.ON));
                var onState = layer.AddNewState("ON").Position(300, 300).CreateClip(Name("ON"), clip => clip.Active(ContactPath, true).Position(ContactPath, ONPosition)).AddParameterDriver(SetFalse(ParameterName.OFF));
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
                var offState = layer.AddNewState("OFF").Position(0, 100).CreateClip(Name("OFF"), clip => clip.Active(ContactPath, true).Position(ContactPath, OFFPosition));
                var onState = layer.AddNewState("ON").Position(0, 200).CreateClip(Name("ON"), clip => clip.Active(ContactPath, true).Position(ContactPath, ONPosition));
                layer.ExitPosition(0, 300);
                layer.DefaultState(offState);
                offState.AddTransition(onState).If(ParameterName.ON);
                onState.AddExitTransition().IfNot(ParameterName.ON);
            }
        }
    }
}
