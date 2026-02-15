using ElPrado.Data.Repositories;

namespace ElPrado.Data
{
    public interface IUnitOfWork : IDisposable
    {
        CampaniasRepository Campanias { get; }
        ClientesRepository Clientes { get; }
        ComprobantesRepository Comprobantes { get; }
        ConfiguracionGeneralRepository ConfiguracionGeneral { get; }
        ContratosRepository Contratos { get; }
        CuentasCorrientesRepository CuentasCorrientes { get; }

        DomiciliosRepository Domicilios { get; }
        InhumadosRepository Inhumados { get; }

        LogsRepository LogsRepository { get; }
        MediosCobrosRepository MediosCobros { get; }
        MercadoPagoRepository MercadoPago { get; }
        MovimientosFondosRepository MovimientosFondos { get; }
        ObituariosRepository Obituarios { get; }

        ParcelasRepository Parcelas { get; }
        PlanesVentasRepository PlanesVentas { get; }
        PropuestasRepository Propuestas { get; }
        ProcesosSistemasRepository ProcesosSistemas { get; }
        RefreshTokensRepository RefreshTokens { get; }
        ServiciosModelosRepository ServiciosModelos { get; }

        UsuariosRepository Usuarios { get; }
        VariosSeguridadRepository VariosSeguridad { get; }

        void Commit();
        void Rollback();
    }
}
