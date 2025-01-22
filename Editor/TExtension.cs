using nadena.dev.ndmf.localization;
using UnityEngine;

namespace Narazaka.VRChat.ContactSync.Editor
{
    internal static class TExtension
    {
        // helper
        public static GUIContent GUIContent(this string text) => new GUIContent(text);
        public static string L(this (string en, string ja) data) => IsJa ? data.ja : data.en;
        static bool IsJa => LanguagePrefs.Language == "ja-jp";
    }
}
