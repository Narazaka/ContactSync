using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace Narazaka.VRChat.ContactSync.Editor
{
    public static class BlendTreeUtil
    {
        public static BlendTree AddDirectChildClip(this BlendTree blendTree, string name, string parameterName, System.Action<AnimationClip> action)
        {
            var clip = new AnimationClip { name = name };
            action(clip);
            var child = new ChildMotion
            {
                motion = clip,
                directBlendParameter = parameterName,
            };
            var children = blendTree.children;
            ArrayUtility.Add(ref children, child);
            blendTree.children = children;
            return blendTree;
        }

        public static BlendTree Add1DChildClip(this BlendTree blendTree, string name, float threshold, System.Action<AnimationClip> action)
        {
            var clip = new AnimationClip { name = name };
            action(clip);
            var child = new ChildMotion
            {
                motion = clip,
                threshold = threshold,
            };
            var children = blendTree.children;
            ArrayUtility.Add(ref children, child);
            blendTree.children = children;
            return blendTree;
        }
    }
}
