using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
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

         Task<ParametroLiquidacionTercero> ObtenerParametroLiquidacionXTercero(int terceroId);

         Task<ICollection<CriterioCalculoReteFuente>> ObtenerListaCriterioCalculoReteFuente();

    }
}