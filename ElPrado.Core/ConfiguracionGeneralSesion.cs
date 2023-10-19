namespace ElPrado.Core
{
    public static class ConfiguracionGeneralSesion
    {
        public static int CodUsuario { get; private set; }
        public static int CodCliente { get; private set; }
        public static int CodPropuesta { get; private set; }
        public static string StrConexion { get; set; } = string.Empty;

        public static void Inicializar(int codAcceso, int codPropuesta)
        {
            CodUsuario = codPropuesta == 0 ? codAcceso : 0;
            CodCliente = codPropuesta > 0 ? codAcceso : 0;
            CodPropuesta = codPropuesta;
        }
    }
}