using Narazaka.VRChat.ContactSync.Editor.Generator.Sender;

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
    }
}
