using ElPrado.Data.Repositories;

namespace ElPrado.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ClientesRepository Clientes { get; }
        ConfiguracionGeneralRepository ConfiguracionGeneral { get; }
        DomiciliosRepository Domicilios { get; }
        LogsRepository LogsRepository { get; }
        PropuestasRepository Propuestas { get; }
        RefreshTokensRepository RefreshTokens { get; }
        UsuariosRepository Usuarios { get; }

        void Commit();
        void Rollback();
    }
}