using ElPrado.Core;
using ElPrado.Data.Helpers;
using ElPrado.Data.Interfaces;
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
        #region Repositorio y Mapper
        private RepositoryBaseCrud<TEntidad, TDto>? _repositoryCrud;
        private IMapper<TEntidad, TDto>? _mapper;

        // Lazy-load del mapper
        protected IMapper<TEntidad, TDto> Mapper => _mapper ??= CrearMapper();
        protected RepositoryBaseCrud<TEntidad, TDto> RepositoryCrud => _repositoryCrud ??= CrearRepository();


        // Permite a clases derivadas devolver su mapper y repository concreto (por defecto null y se lanza si se intenta usar sin configurar)
        protected virtual IMapper<TEntidad, TDto> CrearMapper()
        {
            throw new NotImplementedException("Debe sobrescribir CrearMapper() para devolver un mapper concreto.");
        }
        protected virtual RepositoryBaseCrud<TEntidad, TDto> CrearRepository()
        {
            throw new NotImplementedException("Debe sobrescribir CrearRepository() para devolver un repositorio concreto.");
        }
        #endregion

        public ServiceBaseCrud(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }


        public TDto? Visualizar(int id)
        {
            return RepositoryCrud.Visualizar(id);
        }

        public Resultados<TDto> Modificar(TDto dto)
        {
            Resultados<TDto> resultado = new();

            if (dto == null)
            {
                resultado.Agregar("DTO inválido");
                return resultado;
            }

            // Intentar obtener el ID actualizado desde entidad (asumiendo que tiene una propiedad con [Key]
            object? pkValue = EntityKeyHelper.GetKeyValue(dto);
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

            TEntidad entidad = RepositoryCrud.Buscar(id);
            if (entidad == null)
            {
                resultado.Agregar("La entidad no existe");
                return resultado;
            }

            // Aplicar cambios desde DTO usando el mapper
            Mapper.ApplyToEntity(entidad, dto);

            try
            {
                // Persistir cambios
                RepositoryCrud.Modificar(entidad);
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                throw;
            }


            // Recuperar DTO actualizado desde repositorio (si lo provee)
            TDto? dtoActualizado = RepositoryCrud.Visualizar(id);
            resultado.Valor = dtoActualizado;

            return resultado;
        }

        public Resultados<TDto> Agregar(TDto dto)
        {
            Resultados<TDto> resultado = new();

            if (dto == null)
            {
                resultado.Agregar("DTO inválido");
                return resultado;
            }

            // Valido los datos
            Resultados resultadoAgregar = ValidarAgregar(dto);
            if (resultadoAgregar.HayError)
            {
                resultado.Agregar(resultadoAgregar);
                return resultado;
            }

            // Mapear DTO a entidad
            TEntidad entidad = Mapper.MapToEntity(dto);

            int? id;
            try
            {
                // Persistir nueva entidad (se asume que la BBDD asigna la PK)
                id = AgregarEntidad(entidad);
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                throw;
            }

            if (id != null)
            {
                // Recuperar y devolver el DTO creado si el repositorio lo provee
                TDto? dtoCreado = RepositoryCrud.Visualizar(id.Value);
                resultado.Valor = dtoCreado;
            }

            return resultado;
        }

        protected virtual Resultados ValidarAgregar(TDto dto)
        {
            return new Resultados();
        }

        protected virtual int? AgregarEntidad(TEntidad entidad)
        {
            return RepositoryCrud.Agregar(entidad);
        }

        public Resultados Eliminar(int id)
        {
            Resultados resultado = new();
            try
            {
                RepositoryCrud.Eliminar(id);
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback(); 
                throw;
            }
            return resultado;
        }

        public ApiResponse<IEnumerable<dynamic>> Listado(DtoOpcionesListados opcionesListado)
        {
            return RepositoryCrud.Listado(opcionesListado);
        }

    }
}
