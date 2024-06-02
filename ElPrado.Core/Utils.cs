using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

        public static string ExtraerDigitos(string texto)
        {
            // Utiliza una expresión regular para encontrar todos los dígitos en el texto
            MatchCollection matches = Regex.Matches(texto, @"\d");

            // Concatena todos los dígitos encontrados en una nueva cadena
            string resultado = "";
            foreach (Match match in matches)
            {
                resultado += match.Value;
            }

            return resultado;
        }
    }
}
