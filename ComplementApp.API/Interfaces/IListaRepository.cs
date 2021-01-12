using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IListaRepository
    {
        Task<IEnumerable<Cargo>> ObtenerCargos();

        Task<IEnumerable<Area>> ObtenerAreas();

        Task<IEnumerable<TipoOperacion>> ObtenerListaTipoOperacion();

        Task<IEnumerable<TipoDetalleCDP>> ObtenerListaTipoDetalleModificacion();

        Task<IEnumerable<Tercero>> ObtenerListaTercero(string numeroIdentificacion);

        Task<IEnumerable<Perfil>> ObtenerListaPerfiles();

        Task<IEnumerable<ParametroGeneral>> ObtenerParametrosGenerales();

        Task<ValorSeleccion> ObtenerParametroGeneralXNombre(string nombre);

        Task<ICollection<ValorSeleccion>> ObtenerParametrosGeneralesXTipo(string tipo);


        Task<ICollection<CriterioCalculoReteFuente>> ObtenerListaCriterioCalculoReteFuente();

        Task<IEnumerable<UsuarioParaDetalleDto>> ObtenerListaUsuarioxFiltro(string nombres);

        Task<ICollection<Estado>> ObtenerListaEstado(string tipoDocumento);

        Task<IEnumerable<UsoPresupuestal>> ObtenerListaUsoPresupuestalXRubro(int rubroPresupuestalId);

        Task<ICollection<ValorSeleccion>> ObtenerListaXTipo(TipoLista tipo);

        Task<IEnumerable<Deduccion>> ObtenerListaDeducciones(string codigo);

        Task<IEnumerable<ActividadEconomica>> ObtenerListaActividadesEconomicas(string codigo);

        
    }
}