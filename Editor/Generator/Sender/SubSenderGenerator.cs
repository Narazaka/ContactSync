using Narazaka.VRChat.ContactSync.Editor.NameProvider;
using Narazaka.VRChat.ContactSync.Editor.ParameterProvider;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

namespace Narazaka.VRChat.ContactSync.Editor.Generator.Sender
{
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
            var contact = obj.AddComponent<VRCContactSender>();
            contact.shapeType = ContactBase.ShapeType.Sphere;
            contact.radius = Constant.SenderContactRadius;
            contact.collisionTags = CollisionTags;
            return contact;
        }
    }
}
