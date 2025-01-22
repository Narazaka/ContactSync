using System.Linq;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    abstract class CommunicatorGenerator<T> : IGenerator where T : ContactSyncCommunicator
    {
        protected readonly ContactSyncGenerator ContactSyncGenerator;
        protected readonly T Com;
        protected readonly ContactSyncTagGroup TagGroup;
        protected readonly Tag[] Tags;
        protected readonly Tag Tag;
        protected readonly int TagIndex;

        public CommunicatorGenerator(ContactSyncGenerator gen, T com)
        {
            ContactSyncGenerator = gen;
            Com = com;
            TagGroup = ContactSyncGenerator.Assign.ContactSyncTagGroup;
            Tags = TagGroup.Tags;
            Tag = Tags.First(tag => tag.Name == Com.TagName);
            TagIndex = System.Array.IndexOf(Tags, Tag);
        }

        public abstract void Generate();

        protected S GetSubGenerator<S>() where S : CommunicatorSubGenerator<T>
        {
            return (S)System.Activator.CreateInstance(typeof(S), ContactSyncGenerator, Com);
        }
    }
}
