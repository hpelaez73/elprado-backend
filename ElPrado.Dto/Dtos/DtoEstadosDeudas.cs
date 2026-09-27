namespace ElPrado.Dto.Dtos
{
    /// <summary>Hechos funcionales de un estado configurable de deuda.</summary>
    public class DtoEstadoDeuda
    {
        public string Nombre { get; set; } = string.Empty;
        public bool Activa { get; set; }
        public bool? AlDia { get; set; }
        public bool PermiteImputar { get; set; }
    }

    /// <summary>Estado funcional de una cuenta identificado por su origen y código.</summary>
    public class DtoEstadoDeudaCuenta : DtoEstadoDeuda
    {
        public string Tipo { get; set; } = string.Empty;
        public int Codigo { get; set; }
    }
}
