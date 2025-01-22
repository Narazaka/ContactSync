using UnityEngine;
using nadena.dev.modular_avatar.core;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Narazaka.VRChat.ContactSync.Editor
{
    public static class MenuUtil
    {
        public static GameObject CreateMenu(this GameObject parent, MenuItem menuItem, VRCExpressionsMenu.Control.ControlType type, string parameterName, bool value, string defaultName)
        {
            return CreateMenu(menuItem, type, parameterName, value, defaultName).SetParentZero(parent);
        }

        public static GameObject CreateMenu(MenuItem menuItem, VRCExpressionsMenu.Control.ControlType type, string parameterName, bool value, string defaultName)
        {
            return CreateMenu(menuItem, type, parameterName, value.ToFloat(), defaultName);
        }

        public static GameObject CreateMenu(this GameObject parent, MenuItem menuItem, VRCExpressionsMenu.Control.ControlType type, string parameterName, float value, string defaultName)
        {
            return CreateMenu(menuItem, type, parameterName, value, defaultName).SetParentZero(parent);
        }

        public static GameObject CreateMenu(MenuItem menuItem, VRCExpressionsMenu.Control.ControlType type, string parameterName, float value, string defaultName)
        {
            var name = string.IsNullOrEmpty(menuItem.Name) ? defaultName : menuItem.Name;
            var obj = new GameObject(name);
            var ma = obj.AddComponent<ModularAvatarMenuItem>();
            ma.Control = new VRCExpressionsMenu.Control
            {
                name = name,
                type = type,
                icon = menuItem.Icon,
                parameter = new VRCExpressionsMenu.Control.Parameter
                {
                    name = parameterName
                },
                value = value,
                subParameters = new VRCExpressionsMenu.Control.Parameter[0],
                labels = new VRCExpressionsMenu.Control.Label[0],
            };
            return obj;
        }

        public static GameObject CreateRadialMenu(this GameObject parent, MenuItem menuItem, string parameterName, string defaultName)
        {
            return CreateRadialMenu(menuItem, parameterName, defaultName).SetParentZero(parent);
        }

        public static GameObject CreateRadialMenu(MenuItem menuItem, string parameterName, string defaultName)
        {
            var name = string.IsNullOrEmpty(menuItem.Name) ? defaultName : menuItem.Name;
            var obj = new GameObject(name);
            var ma = obj.AddComponent<ModularAvatarMenuItem>();
            ma.Control = new VRCExpressionsMenu.Control
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
                icon = menuItem.Icon,
                parameter = new VRCExpressionsMenu.Control.Parameter { name = "" },
                subParameters = new VRCExpressionsMenu.Control.Parameter[]
                {
                        new VRCExpressionsMenu.Control.Parameter
                        {
                            name = parameterName,
                        },
                },
                labels = new VRCExpressionsMenu.Control.Label[0],
            };
            return obj;
        }

        public static GameObject CreateParentMenu(this GameObject parent, MenuItem menuItem, string defaultName)
        {
            return CreateParentMenu(menuItem, defaultName).SetParentZero(parent);
        }

        public static GameObject CreateParentMenu(MenuItem menuItem, string defaultName)
        {
            var name = string.IsNullOrEmpty(menuItem.Name) ? defaultName : menuItem.Name;
            var obj = new GameObject(name);
            var ma = obj.AddComponent<ModularAvatarMenuItem>();
            ma.Control = new VRCExpressionsMenu.Control
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                icon = menuItem.Icon,
                parameter = new VRCExpressionsMenu.Control.Parameter { name = "" },
                subParameters = new VRCExpressionsMenu.Control.Parameter[0],
                labels = new VRCExpressionsMenu.Control.Label[0],
            };
            ma.MenuSource = SubmenuSource.Children;
            return obj;
        }
    }
}
