using System.Security.Cryptography;

namespace DemoMsUser.Common.Helpers
{
    public static class StringHelper
    {
        public static string RandomString()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static bool IsAllNullOrEmpty(params string?[] args) => args.All(string.IsNullOrEmpty);
    }
}
