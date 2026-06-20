namespace Afip.Data.Models;

public class AfipConfiguraciones
{
    public string AfipToken { get; set; } = string.Empty;
    public string AfipSign { get; set; } = string.Empty;
    public DateTime AfipGeneracion { get; set; }
    public DateTime AfipExpiracion { get; set; }
    public uint AfipIdUnico { get; set; }
}
