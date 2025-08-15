using ElPrado.Data.Repositories;
using Microsoft.Extensions.Configuration;

namespace ElPrado.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;

        private ClientesRepository? _clientesRepository;
        private ConfiguracionGeneralRepository? _configuracionGeneralRepository;
        private DomiciliosRepository? _domiciliosRepository;
        private LogsRepository? _logsRepository;
        private PropuestasRepository? _propuestasRepository;
        private RefreshTokensRepository? _refreshTokensRepository;
        private UsuariosRepository? _usuariosRepository;

        public UnitOfWork(IConfiguration configuration)
        {
            _dbContext = new DbContext(configuration);
        }

        public ClientesRepository Clientes => _clientesRepository ??= new ClientesRepository(_dbContext);
        public ConfiguracionGeneralRepository ConfiguracionGeneral => _configuracionGeneralRepository ??= new ConfiguracionGeneralRepository(_dbContext);
        public DomiciliosRepository Domicilios => _domiciliosRepository ??= new DomiciliosRepository(_dbContext);
        public LogsRepository LogsRepository => _logsRepository ??= new LogsRepository(_dbContext);
        public PropuestasRepository Propuestas => _propuestasRepository ??= new PropuestasRepository(_dbContext);
        public RefreshTokensRepository RefreshTokens => _refreshTokensRepository ??= new RefreshTokensRepository(_dbContext);
        public UsuariosRepository Usuarios => _usuariosRepository ??= new UsuariosRepository(_dbContext);


        public void Commit()
        {
            _dbContext.Commit();
        }

        public void Rollback()
        {
            _dbContext.Rollback();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
