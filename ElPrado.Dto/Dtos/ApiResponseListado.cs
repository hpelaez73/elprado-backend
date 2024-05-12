namespace ElPrado.Dto.Dtos
{
    public class ApiResponseListado<T> : ApiResponse<T>
    {
        public int CantidadPaginas { get; set; }
        public int CantidadRegistros { get; set; }
        public List<DtoCamposListado> ListFiltros { get; set; } = new();
    }
}
