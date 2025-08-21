using ElPrado.Data.Repositories;
using Microsoft.Extensions.Configuration;

namespace ElPrado.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;

        private CampaniasRepository? _campaniasRepository;
        private ClientesRepository? _clientesRepository;
        private ComprobantesRepository? _comprobantesRepository;
        private ConfiguracionGeneralRepository? _configuracionGeneralRepository;
        private ContratosRepository? _contratosRepository;
        private CuentasCorrientesRepository? _cuentasCorrientesRepository;

        private DomiciliosRepository? _domiciliosRepository;
        private InhumadosRepository? _inhumadosRepository;

        private LogsRepository? _logsRepository;
        private MediosCobrosRepository? _mediosCobrosRepository;
        private MercadoPagoRepository? _mercadoPagoRepository;
        private MovimientosFondosRepository? _movimientosFondosRepository;

        private ParcelasRepository? _parcelasRepository;
        private PlanesVentasRepository? _planesVentasRepository;
        private PropuestasRepository? _propuestasRepository;
        private ProcesosSistemasRepository? _procesosSistemasRepository;
        private RefreshTokensRepository? _refreshTokensRepository;
        private ServiciosModelosRepository? _serviciosModelosRepository;

        private UsuariosRepository? _usuariosRepository;
        private VariosSeguridadRepository? _variosSeguridadRepository;

        public UnitOfWork(IConfiguration configuration)
        {
            _dbContext = new DbContext(configuration);
        }

        public CampaniasRepository Campanias => _campaniasRepository ?? new CampaniasRepository(_dbContext);
        public ClientesRepository Clientes => _clientesRepository ??= new ClientesRepository(_dbContext);
        public ComprobantesRepository Comprobantes => _comprobantesRepository ??= new ComprobantesRepository(_dbContext);
        public ConfiguracionGeneralRepository ConfiguracionGeneral => _configuracionGeneralRepository ??= new ConfiguracionGeneralRepository(_dbContext);
        public ContratosRepository Contratos => _contratosRepository ??= new ContratosRepository(_dbContext);
        public CuentasCorrientesRepository CuentasCorrientes => _cuentasCorrientesRepository ??= new CuentasCorrientesRepository(_dbContext);

        public DomiciliosRepository Domicilios => _domiciliosRepository ??= new DomiciliosRepository(_dbContext);
        public InhumadosRepository Inhumados => _inhumadosRepository ??= new InhumadosRepository(_dbContext);

        public LogsRepository LogsRepository => _logsRepository ??= new LogsRepository(_dbContext);
        public MediosCobrosRepository MediosCobros => _mediosCobrosRepository ??= new MediosCobrosRepository(_dbContext);
        public MercadoPagoRepository MercadoPago => _mercadoPagoRepository ??= new MercadoPagoRepository(_dbContext);
        public MovimientosFondosRepository MovimientosFondos => _movimientosFondosRepository ??= new MovimientosFondosRepository(_dbContext);

        public ParcelasRepository Parcelas => _parcelasRepository ??= new ParcelasRepository(_dbContext);
        public PlanesVentasRepository PlanesVentas => _planesVentasRepository ??= new PlanesVentasRepository(_dbContext);
        public PropuestasRepository Propuestas => _propuestasRepository ??= new PropuestasRepository(_dbContext);
        public ProcesosSistemasRepository ProcesosSistemas => _procesosSistemasRepository ??= new ProcesosSistemasRepository(_dbContext);
        public RefreshTokensRepository RefreshTokens => _refreshTokensRepository ??= new RefreshTokensRepository(_dbContext);
        public ServiciosModelosRepository ServiciosModelos => _serviciosModelosRepository ??= new ServiciosModelosRepository(_dbContext);

        public UsuariosRepository Usuarios => _usuariosRepository ??= new UsuariosRepository(_dbContext);
        public VariosSeguridadRepository VariosSeguridad => _variosSeguridadRepository ??= new VariosSeguridadRepository(_dbContext);


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
