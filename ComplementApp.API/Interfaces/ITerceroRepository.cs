using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface ITerceroRepository
    {
        Task<TerceroDto> ObtenerTercero(int terceroId);
        Task<Tercero> ObtenerTerceroBase(int terceroId);
        Task<PagedList<TerceroDto>> ObtenerTerceros(int? terceroId, UserParams userParams);
        Task<bool> ValidarExistenciaTercero(int TipoIdentificacion, string numeroIdentificacion);
        Task<PagedList<TerceroDto>> ObtenerTercerosParaParametrizacionLiquidacion(int tipo, int? terceroId, UserParams userParams);
        Task<ParametroLiquidacionTerceroDto> ObtenerParametrizacionLiquidacionXTercero(int terceroId);
        Task<ParametroLiquidacionTercero> ObtenerParametrizacionLiquidacionTerceroBase(int parametroLiquidacionTerceroId);
        Task<ICollection<TerceroDeduccionDto>> ObtenerDeduccionesXTercero(int terceroId);
        Task<PagedList<DeduccionDto>> ObteneListaDeducciones(UserParams userParams);
        Task<bool> EliminarTerceroDeduccionesXTercero(int terceroId);
        Task<ICollection<ValorSeleccion>> ObtenerListaActividadesEconomicaXTercero(int terceroId);
        List<int> ObtenerTercerosConMasDeUnaActividadEconomica();
        Task<ParametroLiquidacionTercero> ObtenerParametroLiquidacionXTercero(int terceroId);
        Task<ICollection<ParametroLiquidacionTercero>> ObtenerListaParametroLiquidacionTerceroXIds(List<int> listaTerceroId);
        Task<ICollection<Deduccion>> ObtenerDeduccionesXTercero(int terceroId, int? actividadEconomicaId);
        Task<ICollection<TerceroDeduccion>> ObtenerListaDeduccionesXTerceroIds(List<int> listaTerceroId);

    }
}