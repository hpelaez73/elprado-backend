using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ElPrado.Core.Utils
{
    public static class FunUtils
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

        public static bool EsTelefonoValido(string nroTelefono)
        {
            if (string.IsNullOrEmpty(nroTelefono)) return false;
            // Expresión regular para validar números de teléfono de Argentina
            string patron = @"^\d{10}$";

            Regex regex = new Regex(patron);
            return regex.IsMatch(nroTelefono);
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

        public static string ImporteEnLetras(double importe)
        {
            // Basado en la función Delphi proporcionada.
            if (double.IsNaN(importe) || double.IsInfinity(importe)) return string.Empty;

            if (importe == 0) return "Cero con 00/100";
            bool negativo = importe < 0;

            // Trabajar con valores absolutos y en centavos para evitar problemas de redondeo
            decimal importeDec = (decimal)importe;
            long totalCentavos = (long)Math.Round(Math.Abs(importeDec) * 100m);
            long entero = totalCentavos / 100;
            int decimales = (int)(totalCentavos % 100);

            // Arrays de unidades/decenas en minúscula (seguimos la lógica Delphi)
            string[] unidad = new string[16] { "", "un", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez", "once", "doce", "trece", "catorce", "quince" };
            string[] decena = new string[10] { "", "dieci", "veinti", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };

            string TranDecena(int D)
            {
                if (D == 0) return string.Empty;
                if (D <= 15) return unidad[D];
                int dece = D / 10;
                int unid = D % 10;
                if (unid == 0)
                {
                    if (dece == 2) return "veinte";
                    return decena[dece];
                }
                else if (dece <= 2)
                    return decena[dece] + unidad[unid];
                else
                    return decena[dece] + " y " + unidad[unid];
            }

            string TranCentena(int C)
            {
                if (C == 0) return string.Empty;
                int cent = C / 100;
                int dece = C % 100;
                string res = string.Empty;
                if (dece > 0) res = TranDecena(dece);
                if (cent == 0) return res;
                return cent switch
                {
                    1 => (dece == 0) ? "cien" : "ciento " + res,
                    5 => "quinientos" + (res.Length > 0 ? " " + res : ""),
                    7 => "setecientos" + (res.Length > 0 ? " " + res : ""),
                    9 => "novecientos" + (res.Length > 0 ? " " + res : ""),
                    _ => unidad[cent] + "cientos" + (res.Length > 0 ? " " + res : ""),
                };
            }

            string TranMil(int M)
            {
                if (M == 0) return string.Empty;
                int cent = M % 1000;
                string res = string.Empty;
                if (cent > 0) res = TranCentena(cent);
                int miles = M / 1000;
                if (miles > 0)
                {
                    if (miles == 1) return "mil" + (res.Length > 0 ? " " + res : "");
                    return TranCentena(miles) + " mil" + (res.Length > 0 ? " " + res : "");
                }
                return res;
            }

            string TranMillon(long N)
            {
                if (N == 0) return string.Empty;
                long mill = N / 1000000;
                long resto = N % 1000000;
                string sMill = string.Empty;
                if (mill == 1) sMill = "un millón";
                else if (mill > 1) sMill = TranMil((int)mill) + " millones";

                if (resto > 0)
                {
                    string sResto = resto < 1000 ? TranCentena((int)resto) : TranMil((int)resto);
                    return sMill + " " + sResto;
                }
                return sMill;
            }

            string texto;
            if (entero == 0) texto = "cero";
            else if (entero <= 99) texto = TranDecena((int)entero);
            else if (entero <= 999) texto = TranCentena((int)entero);
            else if (entero <= 999999) texto = TranMil((int)entero);
            else texto = TranMillon(entero);

            // Capitalizar primera letra y añadir parte decimal
            if (!string.IsNullOrEmpty(texto))
            {
                texto = char.ToUpperInvariant(texto[0]) + texto.Substring(1);
            }

            texto = texto + $" con {decimales:00}/100";
            if (negativo) texto = "Menos " + texto;
            return texto;
        }
    }
}