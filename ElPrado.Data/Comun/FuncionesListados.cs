using Dapper;
using ElPrado.Core;
using ElPrado.Core.Enums;
using ElPrado.Dto.Dtos;
using System.Text;
using System.Text.RegularExpressions;

namespace ElPrado.Data.Comun
{
    internal class FuncionesListados<TDtoList> where TDtoList : DtoBase
    {
        private ConfiguracionListado configuracionListado;
        private DtoOpcionesListados opcionesListados;
        private int cantidadRegistros;
        private int cantidadPaginas;

        public FuncionesListados(ConfiguracionListado configuracion, DtoOpcionesListados? opciones)
        {
            configuracionListado = configuracion;
            opcionesListados = opciones ?? ArmarOpcionesDefault();
        }
        internal ApiResponseListado<IEnumerable<dynamic>> ApiResponse(string sql, System.Data.IDbConnection _connection, System.Data.IDbTransaction _transaction)
        {
            ApiResponseListado<IEnumerable<dynamic>> apiResponse = new();
            if (opcionesListados.MostrarFiltros)
            {
                apiResponse.ListFiltros = MapOpcionesListado();
                return apiResponse;
            }
            apiResponse.CantidadPaginas = cantidadPaginas;
            apiResponse.CantidadRegistros = cantidadRegistros;
            apiResponse.Data = _connection.Query<TDtoList>(sql, null, _transaction);
            return apiResponse;
        }

        private List<DtoCamposListado> MapOpcionesListado()
        {
            return configuracionListado.ListCampos.Select(
                x => new DtoCamposListado()
                {
                    Campo = x.Campo,
                    EndPoint = x.EndPoint,
                    Etiqueta = x.Etiqueta,
                    PermiteFiltrar = x.PermiteFiltrar,
                    PermiteOrdenar = x.PermiteOrdenar,
                    TipoDato = x.TipoDato
                }).ToList();
        }

        private DtoOpcionesListados ArmarOpcionesDefault()
        {
            DtoOpcionesListados opciones = new()
            {
                ListFiltros = new(),
                ListOrden = new()
            };

            foreach (CamposListado item in configuracionListado.ListCampos)
            {
                if (item.PermiteFiltrar && item.FiltroDefault)
                {
                    opciones.ListFiltros.Add(
                        new DtoCamposFiltroListado()
                        {
                            Campo = item.Campo,
                            TipoComparacion = item.TipoComparacionDefault,
                            Valor = item.ValorComparacionDefault
                        }
                    );
                }
                if (item.PermiteOrdenar && item.OrdenDefault)
                {
                    opciones.ListOrden.Add(
                        new DtoCamposOrdenListado()
                        {
                            Campo = item.Campo,
                            Ascendente = item.AscendenteDefault
                        }
                    );
                }
            }

            return opciones;
        }

        internal string ParseSqlWhere(int orden = 1)
        {
            if (opcionesListados.ListFiltros == null || opcionesListados.ListFiltros.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new();
            foreach (DtoCamposFiltroListado item in opcionesListados.ListFiltros)
            {
                if (!item.TipoComparacion.In(TipoComparacion.EnLista, TipoComparacion.EsNulo, TipoComparacion.EsNoNulo)
                    && string.IsNullOrWhiteSpace(item.Valor)) continue;
                if ((item.TipoComparacion == TipoComparacion.Entre)
                    && string.IsNullOrWhiteSpace(item.ValorHasta)) continue;
                if ((item.TipoComparacion == TipoComparacion.EnLista)
                    && (item.ListValores == null || item.ListValores.Count == 0)) continue;

                CamposListado? campo = configuracionListado.ListCampos.Find(x => x.Campo.ToLower() == item.Campo.ToLower());
                if (campo == null) continue;
                if (!campo.PermiteFiltrar) continue;
                if (orden == 1 && string.IsNullOrEmpty(campo.CampoSql)) continue;
                if (orden == 2 && string.IsNullOrEmpty(campo.CampoSql2)) continue;

                if ((campo.TipoDato == TipoDatoListado.Entero)
                    && !string.IsNullOrWhiteSpace(item.Valor) && !Regex.IsMatch(item.Valor, @"^\d+$")) continue;

                // Comienzo a construir el Where
                if (builder.Length > 0) builder.Append(" AND ");

                if (orden == 2) builder.Append((campo.TipoDato == TipoDatoListado.Texto) ? $"UPPER(TRIM({campo.CampoSql2}))" : campo.CampoSql2);
                else builder.Append((campo.TipoDato == TipoDatoListado.Texto) ? $"UPPER(TRIM({campo.CampoSql}))" : campo.CampoSql);

                if (item.TipoComparacion.In(TipoComparacion.Contiene, TipoComparacion.ComienzaCon) && campo.TipoDato != TipoDatoListado.Texto)
                {
                    item.TipoComparacion = TipoComparacion.Igual;
                }
                switch (item.TipoComparacion)
                {
                    case TipoComparacion.Igual:
                        builder.Append(" = ");
                        break;
                    case TipoComparacion.Menor:
                        builder.Append(" < ");
                        break;
                    case TipoComparacion.MenorIgual:
                        builder.Append(" <= ");
                        break;
                    case TipoComparacion.Mayor:
                        builder.Append(" > ");
                        break;
                    case TipoComparacion.MayorIgual:
                        builder.Append(" >= ");
                        break;
                    case TipoComparacion.Distinto:
                        builder.Append(" <> ");
                        break;
                    case TipoComparacion.Entre:
                        builder.Append(" BETWEEN ");
                        break;
                    case TipoComparacion.Contiene:
                        builder.Append(" CONTAINING ");
                        break;
                    case TipoComparacion.ComienzaCon:
                        builder.Append(" STARTING WITH ");
                        break;
                    case TipoComparacion.EnLista:
                        builder.Append(" IN (");
                        item.Valor = item.ListValores![0];
                        break;
                    case TipoComparacion.EsNulo:
                        builder.Append(" IS NULL ");
                        break;
                    case TipoComparacion.EsNoNulo:
                        builder.Append(" IS NOT NULL ");
                        break;
                }
                builder.Append(Valor2Sql(campo.TipoDato, item.TipoComparacion, item.Valor));
                if (item.TipoComparacion == TipoComparacion.Entre)
                {
                    builder.Append(" AND " + Valor2Sql(campo.TipoDato, item.TipoComparacion, item.ValorHasta));
                }
                else if (item.TipoComparacion == TipoComparacion.EnLista)
                {
                    for (int i = 1; i < item.ListValores!.Count; i++)
                    {
                        builder.Append(", " + Valor2Sql(campo.TipoDato, item.TipoComparacion, item.ListValores[i]));
                    }
                    builder.Append(") ");
                }
            }
            if (builder.Length > 0)
            {
                builder.Insert(0, " AND ");
                builder.Append(" ");
            }
            return builder.ToString();
        }

        internal string ParseSqlOrden()
        {
            if (opcionesListados.ListOrden == null || opcionesListados.ListOrden.Count == 0)
            {
                opcionesListados.ListOrden = configuracionListado.ListCampos
                    .Where(x => x.PermiteOrdenar && x.OrdenDefault)
                    .Select(item => new DtoCamposOrdenListado()
                    {
                        Campo = item.Campo,
                        Ascendente = item.AscendenteDefault
                    }).ToList();
            }

            StringBuilder builder = new();
            if (opcionesListados.ListOrden.Count > 0)
            {
                foreach (DtoCamposOrdenListado item in opcionesListados.ListOrden)
                {
                    CamposListado? campo = configuracionListado.ListCampos.Find(x => x.Campo.ToLower() == item.Campo.ToLower() && x.PermiteOrdenar);
                    if (campo != null)
                    {
                        if (builder.Length > 0) builder.Append(", ");
                        builder.Append((campo.CampoSqlOrden > 0 ? campo.CampoSqlOrden.ToString() : campo.CampoSql) + (item.Ascendente ? string.Empty : " DESC"));
                    }
                }
            }
            if (builder.Length > 0) builder.Insert(0, " ORDER BY ");

            return builder.ToString();
        }

        internal string ParseSqlPaginado(string sqlCant, System.Data.IDbConnection _connection, System.Data.IDbTransaction _transaction)
        {
            if (opcionesListados.MostrarFiltros) return string.Empty;

            opcionesListados.FilasPagina = (opcionesListados.FilasPagina == 0) ? 10 : opcionesListados.FilasPagina;
            if (opcionesListados.Pagina <= 1 && !opcionesListados.SinPaginado)
            {
                cantidadRegistros = _connection.QuerySingleOrDefault<int>(sqlCant, null, _transaction);
                cantidadPaginas = cantidadRegistros % opcionesListados.FilasPagina == 0 ? cantidadRegistros / opcionesListados.FilasPagina : cantidadRegistros / opcionesListados.FilasPagina + 1;
            }

            string sqlPagina = string.Empty;
            if (!opcionesListados.SinPaginado || cantidadRegistros > 200)
            {
                int offset = (opcionesListados.Pagina - 1) * opcionesListados.FilasPagina;
                if (opcionesListados.Pagina > 0) sqlPagina = $" FIRST {opcionesListados.FilasPagina} SKIP {offset}";
                else sqlPagina = $" FIRST {opcionesListados.FilasPagina}";
            }
            return sqlPagina;
        }

        private static string Valor2Sql(string tipoDato, TipoComparacion tipoComparacion, string valor)
        {
            if (tipoComparacion.In(TipoComparacion.EsNulo, TipoComparacion.EsNoNulo)) return string.Empty;

            string sql;

            switch (tipoDato)
            {
                case TipoDatoListado.Texto:
                    sql = $"UPPER(TRIM('{valor.Replace("'", "''")}'))";
                    break;
                case TipoDatoListado.Fecha:
                    sql = $"'{DateTime.Parse(valor):MM/dd/yyyy}'";
                    break;
                case TipoDatoListado.Boolean:
                    sql = (valor.ToUpper() == "TRUE" || valor == "1") ? "1" : "0";
                    break;
                case TipoDatoListado.Numero:
                    sql = valor.Replace(',', '.');
                    break;
                default:
                    sql = valor;
                    break;
            }

            return sql;
        }

    }
}
