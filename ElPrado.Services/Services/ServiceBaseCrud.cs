using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Helpers;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class ServiceBaseCrud<TEntidad, TDto> : ServiceBase
        where TEntidad : Entidades, new()
        where TDto : DtoBase
    {
        public required RepositoryBaseCrud<TEntidad, TDto> _repositoryCrud;
        private IMapper<TEntidad, TDto>? _mapper;

        // Lazy-load del mapper
        protected IMapper<TEntidad, TDto> Mapper => _mapper ??= CrearMapper();

        public ServiceBaseCrud(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        // Permite a clases derivadas devolver su mapper concreto (por defecto null y se lanza si se intenta usar sin configurar)
        protected virtual IMapper<TEntidad, TDto> CrearMapper()
        {
            throw new NotImplementedException("Debe sobrescribir CrearMapper() para devolver un mapper concreto.");
        }

        public TDto? Visualizar(int id)
        {
            return _repositoryCrud.Visualizar(id);
        }

        public Resultados<TDto> Actualizar(TDto dto)
        {
            Resultados<TDto> resultado = new();

            if (dto == null)
            {
                resultado.Agregar("DTO inválido");
                return resultado;
            }

            // Aplicar cambios desde DTO usando el mapper
            TEntidad entidad = Mapper.MapToEntity(dto);

            // Persistir cambios
            _repositoryCrud.Modificar(entidad);

            // Intentar obtener el ID actualizado desde entidad (asumiendo que tiene una propiedad con [Key]
            object? pkValue = EntityKeyHelper.GetKeyValue(entidad);
            if (pkValue == null)
            {
                resultado.Agregar("No se pudo determinar la clave primaria de la entidad.");
                return resultado;
            }

            int id;
            try
            {
                id = Convert.ToInt32(pkValue);
            }
            catch
            {
                resultado.Agregar("La clave primaria no es convertible a entero.");
                return resultado;
            }

            // Recuperar DTO actualizado desde repositorio (si lo provee)
            TDto? dtoActualizado = _repositoryCrud.Visualizar(id);
            resultado.Valor = dtoActualizado;

            return resultado;
        }

        public ApiResponse<IEnumerable<dynamic>> Listado(DtoOpcionesListados opcionesListado)
        {
            return (_repositoryCrud != null) ? _repositoryCrud.Listado(opcionesListado) : new ApiResponseListado<IEnumerable<dynamic>>();
        }

    }
}
