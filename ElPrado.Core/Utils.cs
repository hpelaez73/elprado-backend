using System.ComponentModel.DataAnnotations;

namespace ElPrado.Core
{
    public static class Utils
    {
        public static string SHA1(string value)
        {
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(System.Security.Cryptography.SHA1.HashData(combined)).ToLower().Replace("-", "");
        }

        public static bool EsMailValido(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return new EmailAddressAttribute().IsValid(email.Trim());
        }
    }
}
