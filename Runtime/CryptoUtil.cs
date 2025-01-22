namespace Narazaka.VRChat.ContactSync
{
    public static class CryptoUtil
    {
        public static string RandomHash()
        {
            var bytes = new byte[12];
            var rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
            rnd.GetBytes(bytes);
            return SafeBase64(bytes);
        }

        public static string Hash(string tag)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(tag));
            return SafeBase64(bytes);
        }

        static string SafeBase64(byte[] bytes)
        {
            return System.Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-").Replace("=", "");
        }
    }
}
