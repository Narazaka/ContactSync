using UnityEngine;
using UnityEditor;

namespace Narazaka.VRChat.ContactSync.Editor
{
    public static class AnimationClipUtil
    {
        public static AnimationCurve ShortCurve(float value) => new AnimationCurve(new Keyframe { time = 0, value = value }, new Keyframe { time = 1 / 60f, value = value });

        static string EmptyName = "__ContactSync__Empty__";

        public static AnimationClip Empty(this AnimationClip clip) => clip.Active(EmptyName, false);

        public static AnimationClip EmptyClip => _emptyClip ??= new AnimationClip { name = EmptyName }.Empty();

        static AnimationClip _emptyClip;

        public static AnimationClip Active(this AnimationClip clip, string path, bool enable)
        {
            clip.SetCurve(path, typeof(GameObject), "m_IsActive", ShortCurve(enable.ToFloat()));
            return clip;
        }

        public static AnimationClip Position(this AnimationClip clip, string path, Vector3 position)
        {
            clip.SetCurve(path, typeof(Transform), "localPosition.x", ShortCurve(position.x));
            clip.SetCurve(path, typeof(Transform), "localPosition.y", ShortCurve(position.y));
            clip.SetCurve(path, typeof(Transform), "localPosition.z", ShortCurve(position.z));
            return clip;
        }

        public static AnimationClip PositionFromTo(this AnimationClip clip, string path, Vector3 from, Vector3 to)
        {
            clip.SetCurve(path, typeof(Transform), "localPosition.x", LinearCurve(from.x, to.x));
            clip.SetCurve(path, typeof(Transform), "localPosition.y", LinearCurve(from.y, to.y));
            clip.SetCurve(path, typeof(Transform), "localPosition.z", LinearCurve(from.z, to.z));
            return clip;
        }

        public static AnimationCurve LinearCurve(float from, float to)
        {
            var curve = new AnimationCurve(new Keyframe { time = 0, value = from }, new Keyframe { time = 1, value = to });
            AnimationUtility.SetKeyRightTangentMode(curve, 0, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyLeftTangentMode(curve, 1, AnimationUtility.TangentMode.Linear);
            return curve;
        }
    }
}
