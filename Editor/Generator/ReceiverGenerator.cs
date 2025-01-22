using nadena.dev.modular_avatar.core;
using Narazaka.VRChat.ContactSync.Editor.Generator.Sender;
using Narazaka.VRChat.ContactSync.Editor.NameProvider;
using Narazaka.VRChat.ContactSync.Editor.ParameterProvider;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    class ReceiverGenerator : CommunicatorGenerator<ContactSyncReceiver>
    {
        public ReceiverGenerator(ContactSyncGenerator gen, ContactSyncReceiver com) : base(gen, com)
        {
        }

        public override void Generate()
        {
            switch (Tag.ReceiverType)
            {
                case ContactSyncReceiverType.Trigger:
                    GetSubGenerator<TriggerGenerator>().Generate();
                    break;
                case ContactSyncReceiverType.Toggle:
                    GetSubGenerator<ToggleGenerator>().Generate();
                    break;
                case ContactSyncReceiverType.Choose:
                    GetSubGenerator<ChooseGenerator>().Generate();
                    break;
                case ContactSyncReceiverType.Radial:
                    GetSubGenerator<RadialGenerator>().Generate();
                    break;
            }
        }

        abstract class SubReceiverGenerator : CommunicatorSubGenerator<ContactSyncReceiver>
        {
            protected readonly ContactSyncReceiverNameProvider NameProvider;
            protected readonly ContactSyncReceiverNameProvider.ParameterNames ParameterName;
            protected readonly ContactSyncReceiverParameterProvider ParameterProvider;
            protected GameObject Contact;
            protected string ContactPath;
            public SubReceiverGenerator(ContactSyncGenerator gen, ContactSyncReceiver com) : base(gen, com)
            {
                NameProvider = new ContactSyncReceiverNameProvider(com);
                ParameterName = NameProvider.ParameterName;
                ParameterProvider = new ContactSyncReceiverParameterProvider(com);
            }

            protected string Name(string name) => NameProvider.Name(name);
            public override sealed void Generate()
            {
                GenerateParameters();
                Contact = GenerateContact();
                ContactPath = Path(Contact);
                GenerateAnimator();
                if (Component.UseDisableMenu)
                {
                    GenerateDisableMenuParameters();
                    GenerateDisableMenu();
                    GenerateDisableMenuAnimator();
                }
                else
                {
                    GenerateDefaultOnAnimator();
                }
                Object.DestroyImmediate(Component);
            }

            void GenerateDisableMenuParameters()
            {
                var parameters = ParameterProvider.GetMenuParameters();
                AddParameters(parameters);
                AddParametersToController(parameters);
            }
            void GenerateDisableMenu()
            {
                Component.gameObject.AddComponent<ModularAvatarMenuGroup>();
                Component.gameObject.CreateMenu(Component.Menu, VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle, ParameterName.Enabled, Component.Enabled, Component.name);
            }
            void GenerateDisableMenuAnimator()
            {
                var layer = Controller.AddNewLayer(Name("DisableMenu"));
                layer.EntryPosition(0, 0);
                var offState = layer.AddNewState("OFF").Position(0, 100).CreateClip(Name("OFF"), clip => clip.Active(ContactPath, false));
                var onState = layer.AddNewState("ON").Position(0, 200).CreateClip(Name("ON"), clip => clip.Active(ContactPath, true));
                layer.ExitPosition(0, 300);
                layer.DefaultState(offState);
                offState.AddTransition(onState).If(ParameterName.Enabled);
                onState.AddExitTransition().IfNot(ParameterName.Enabled);
            }

            void GenerateDefaultOnAnimator()
            {
                var layer = Controller.AddNewLayer(Name("DefaultOn"));
                layer.EntryPosition(0, 0);
                var onState = layer.AddNewState("ON").Position(0, 100).CreateClip(Name("ON"), clip => clip.Active(ContactPath, true));
                layer.DefaultState(onState);
            }

            protected virtual void GenerateParameters()
            {
                var parameters = ParameterProvider.GetContactParameters();
                AddParameters(parameters);
                AddParametersToController(parameters);
            }

            void GenerateExternalParameters()
            {
                var parameters = ParameterProvider.GetExternalParameters(ContactSyncGenerator.Context);
                AddParametersToController(parameters);
            }

            protected virtual GameObject GenerateContact()
            {
                var contact = GenerateBaseContact(Name(nameof(Contact)));
                contact.receiverType = ContactReceiver.ReceiverType.Proximity;
                contact.parameter = ParameterName.Contact;
                contact.gameObject.SetActive(false);
                return contact.gameObject;
            }

            protected abstract void GenerateAnimator();

            protected VRCContactReceiver GenerateBaseContact(string objectName)
            {
                var obj = Container.CreateGameObjectZero(objectName);
                obj.transform.localPosition = TagLocalPositionBase;
                var contact = obj.AddComponent<VRCContactReceiver>();
                contact.shapeType = ContactBase.ShapeType.Sphere;
                contact.radius = 1f;
                contact.collisionTags = CollisionTags;
                contact.allowSelf = ContactSyncGenerator.Assign.AllowSelfContact;
                contact.allowOthers = true;
                contact.localOnly = Component.LocalOnly;
                contact.minVelocity = Tag.MinVelocity;
                return contact;
            }
        }

        class TriggerGenerator : SubReceiverGenerator
        {
            public TriggerGenerator(ContactSyncGenerator gen, ContactSyncReceiver com) : base(gen, com)
            {
            }

            protected override GameObject GenerateContact()
            {
                var contact = GenerateBaseContact(Name(nameof(Contact)));
                contact.receiverType = ContactReceiver.ReceiverType.Constant;
                contact.parameter = Component.ParameterName;
                contact.gameObject.SetActive(false);
                return contact.gameObject;
            }

            protected override void GenerateAnimator()
            {
            }
        }

        class ToggleGenerator : SubReceiverGenerator
        {
            public ToggleGenerator(ContactSyncGenerator gen, ContactSyncReceiver com) : base(gen, com)
            {
            }

            protected override void GenerateAnimator()
            {
                var layer = Controller.AddNewLayer(Name(nameof(Contact)));
                layer.EntryPosition(-300, 200);
                layer.ExitPosition(300, 200);
                var idleState = layer.AddNewState("Idle").Position(0, 100);
                var offState = layer.AddNewState("Off").Position(0, 200).AddParameterDriver(Set(false));
                var onState = layer.AddNewState("On").Position(0, 300).AddParameterDriver(Set(true));
                layer.DefaultState(idleState);
                // layer.AddEntryTransition(idleState).Less(ParameterName.Contact, FloatPrecision);
                idleState.AddExitTransition().Greater(ParameterName.Contact, Constant.FloatPrecision);
                layer.AddEntryTransition(offState).Greater(ParameterName.Contact, Constant.FloatPrecision).Less(ParameterName.Contact, 0.5f);
                offState.AddExitTransition().AutoExit();
                layer.AddEntryTransition(onState).Greater(ParameterName.Contact, 0.5f);
                onState.AddExitTransition().AutoExit();
            }

            VRC.SDKBase.VRC_AvatarParameterDriver.Parameter Set(bool value) => new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter
            {
                type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set,
                name = Component.ParameterName,
                value = value ? 1 : 0,
            };
        }

        class ChooseGenerator : SubReceiverGenerator
        {
            public ChooseGenerator(ContactSyncGenerator gen, ContactSyncReceiver com) : base(gen, com)
            {
            }

            protected override void GenerateAnimator()
            {
                var layer = Controller.AddNewLayer(Name(nameof(Contact)));
                layer.EntryPosition(-300, 0);
                layer.ExitPosition(300, 0);

                var idleState = layer.AddNewState("Idle").Position(0, -100);
                layer.AddEntryTransition(idleState).Less(ParameterName.Contact, Constant.FloatPrecision);
                idleState.AddExitTransition().Greater(ParameterName.Contact, Constant.FloatPrecision);
                layer.DefaultState(idleState);
                for (var i = 0; i < ChooseConstant.MaxChoiceCount; i++)
                {
                    var contactValue = ChooseConstant.MinContactValue + ChooseConstant.ContactValueStep * i;
                    var (min, max) = (contactValue - ChooseConstant.HalfContactValueStep, contactValue + ChooseConstant.HalfContactValueStep);
                    var state = layer.AddNewState($"Choice {i}").Position(0, i * 100).AddParameterDriver(Set(i));
                    layer.AddEntryTransition(state).Greater(ParameterName.Contact, min).Less(ParameterName.Contact, max);
                    state.AddExitTransition().Less(ParameterName.Contact, min);
                    state.AddExitTransition().Greater(ParameterName.Contact, max);
                }
            }

            VRC.SDKBase.VRC_AvatarParameterDriver.Parameter Set(int value) => new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter
            {
                type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set,
                name = Component.ParameterName,
                value = value,
            };
        }

        class RadialGenerator : SubReceiverGenerator
        {
            public RadialGenerator(ContactSyncGenerator gen, ContactSyncReceiver com) : base(gen, com)
            {
            }

            protected override void GenerateAnimator()
            {
                var layer = Controller.AddNewLayer(Name(nameof(Contact)));
                layer.EntryPosition(0, 0);
                var idleState = layer.AddNewState("Idle").Position(0, -100);
                var valueState = layer.AddNewState("value").Position(0, -200).AddParameterDriver(Copy());
                layer.ExitPosition(300, 0);
                layer.DefaultState(idleState);
                idleState.AddTransition(valueState).Greater(ParameterName.Contact, Constant.FloatPrecision);
                valueState.AddExitTransition().AutoExit();
            }

            VRC.SDKBase.VRC_AvatarParameterDriver.Parameter Copy() => new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter
            {
                type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Copy,
                name = Component.ParameterName,
                source = ParameterName.Contact,
                convertRange = true,
                sourceMin = RadialConstant.MinContactValue,
                sourceMax = RadialConstant.MaxContactValue,
                destMin = 0,
                destMax = 1,
            };
        }
    }
}
