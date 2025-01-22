using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor.Generator
{
    abstract class CommunicatorSubGenerator<T> : SubGenerator where T : ContactSyncCommunicator
    {
        protected readonly T Component;
        protected readonly ContactSyncTagGroup TagGroup;
        protected readonly Tag[] Tags;
        protected readonly Tag Tag;
        protected readonly int TagIndex;
        public CommunicatorSubGenerator(ContactSyncGenerator gen, T com) : base(gen)
        {
            Component = com;
            TagGroup = ContactSyncGenerator.Assign.ContactSyncTagGroup;
            Tags = TagGroup.Tags;
            Tag = Tags.First(tag => tag.Name == Component.TagName);
            TagIndex = System.Array.IndexOf(Tags, Tag);
        }

        protected Vector3 TagLocalPositionBase => Vector3.down * TagIndex * 3;
        protected string CollisionTag => Component.EffectiveTagBase(TagGroup);
        protected List<string> CollisionTags => new List<string> { CollisionTag };
    }
}
