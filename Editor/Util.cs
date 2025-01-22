using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Narazaka.VRChat.ContactSync.Editor
{
    public static class Util
    {
        public static string RelativePath(Transform from, Transform to)
        {
            if (from == to) return "";
            if (from == null) return "";
            if (to == null) return "";
            return string.Join("/", to.GetParents(from).Reverse().Select(t => t.name));
        }
        public static string RelativeDirname(Transform from, Transform to, bool appendTrailingSlash = false)
        {
            if (from == to) return "";
            if (from == null) return "";
            if (to == null) return "";
            var path = string.Join("/", to.GetParents(from).Skip(1).Reverse().Select(t => t.name));
            if (appendTrailingSlash && path != "") path += "/";
            return path;
        }

        public static IEnumerable<Transform> GetParents(this Transform transform, Transform parent)
        {
            while (transform != null)
            {
                yield return transform;
                transform = transform.parent;
                if (transform == parent) break;
            }
        }

        public static int NullableBoolValue(this float value) => value > 0.5f ? 1 : value < -0.5f ? -1 : 0;
        public static bool ToBool(this float value) => value > 0.5f;
        public static float ToFloat(this bool value) => value ? 1f : 0f;

        public static GameObject CreateGameObjectZero(this Transform parent, string name) => new GameObject(name).SetParentZero(parent);
        public static GameObject CreateGameObjectZero(this GameObject parent, string name) => new GameObject(name).SetParentZero(parent);

        public static GameObject SetParentZero(this GameObject gameObject, Transform parent)
        {
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
            return gameObject;
        }

        public static GameObject SetParentZero(this GameObject gameObject, GameObject parent) => gameObject.SetParentZero(parent.transform);
        // public static GameObject SetParentZero(this GameObject gameObject, Component parent) => gameObject.SetParentZero(parent.transform);
    }
}
