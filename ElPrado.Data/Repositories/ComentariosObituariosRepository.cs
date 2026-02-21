using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ComentariosObituariosRepository : RepositoryBaseCrud<ComentariosObituarios, DtoComentariosObituarios>
    {
        public ComentariosObituariosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override DtoComentariosObituarios? Visualizar(int id)
        {
            string sql = "SELECT * FROM COMENTARIOS_OBITUARIOS WHERE COD_COMENTARIO = @id";
            return _connection.QueryFirstOrDefault<DtoComentariosObituarios>(sql, new { id }, _transaction);
        }

        public List<DtoComentariosObituarios> BuscarPorObituario(int id)
        {
            string sql = "SELECT * FROM COMENTARIOS_OBITUARIOS C WHERE C.COD_OBITUARIO = @id";
            return _connection.Query<DtoComentariosObituarios>(sql, new { id }, _transaction).ToList();
        }

        public void ActualizarImagen(int codComentario, int slot, string urlImagen)
        {
            string nombreCampo = slot switch
            {
                1 => "URL_IMAGEN1",
                2 => "URL_IMAGEN2",
                3 => "URL_IMAGEN3",
                _ => throw new ArgumentException("Slot de imagen inválido")
            };

            string sql = $@" UPDATE COMENTARIOS_OBITUARIOS C SET C.{nombreCampo} = @urlImagen
                             WHERE C.COD_COMENTARIO = @codComentario";
            _connection.Execute(sql, new { codComentario, urlImagen }, _transaction);
        }
    }
}
