using UnityEngine;

namespace Narazaka.VRChat.ContactSync
{
#pragma warning disable IDE1006 // 命名スタイル
    public class istring
#pragma warning restore IDE1006 // 命名スタイル
    {
        public string en;
        public string ja;
        public istring(string en, string ja)
        {
            this.en = en;
            this.ja = ja;
        }
        public GUIContent GUIContent => new GUIContent(this);

        public static implicit operator string(istring data) => IsJa ? data.ja : data.en;

        static bool IsJa =>
#if UNITY_EDITOR
            nadena.dev.ndmf.localization.LanguagePrefs.Language == "ja-jp";
#else
            false;
#endif
    }
}
