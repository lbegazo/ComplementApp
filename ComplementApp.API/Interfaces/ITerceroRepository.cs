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
        Task<ICollection<TerceroDto>> ObtenerListaTercero();
        Task<PagedList<TerceroDto>> ObtenerTercerosParaParametrizacionLiquidacion(int tipo, int? terceroId, UserParams userParams);
        Task<ParametroLiquidacionTerceroDto> ObtenerParametrizacionLiquidacionXTercero(int terceroId, int pciId);
        Task<ParametroLiquidacionTercero> ObtenerParametrizacionLiquidacionTerceroBase(int parametroLiquidacionTerceroId);
        Task<ICollection<TerceroDeduccionDto>> ObtenerDeduccionesXTercero(int parametroLiquidacionId);
        Task<PagedList<DeduccionDto>> ObteneListaDeducciones(UserParams userParams);
        Task<bool> EliminarTerceroDeduccionesXTercero(int terceroId, int pciId);
        Task<bool> EliminarTerceroDeduccion(int terceroDeduccionId);
        Task<TerceroDeduccion> ObtenerTerceroDeduccionBase(int terceroDeduccionId);
        Task<ICollection<ValorSeleccion>> ObtenerListaActividadesEconomicaXTercero(int terceroId, int pciId);
        List<int> ObtenerTercerosConMasDeUnaActividadEconomica();
        Task<ParametroLiquidacionTercero> ObtenerParametroLiquidacionXTercero(int terceroId, int pciId);
        Task<ICollection<ParametroLiquidacionTercero>> ObtenerListaParametroLiquidacionTerceroXIds(List<int> listaTerceroId, int pciId);
        Task<ICollection<Deduccion>> ObtenerDeduccionesXTercero(int terceroId,  int pciId, int? actividadEconomicaId);
        Task<ICollection<TerceroDeduccion>> ObtenerListaDeduccionesXTerceroIds(List<int> listaTerceroId, int pciId);
        Task<ICollection<ParametroLiquidacionTerceroDto>> ObtenerListaParametroLiquidacionTerceroTotal(int pciId);
        Task<ICollection<TerceroDeduccionDto>> ObtenerListaTerceroDeduccionTotal(int pciId);
        Task<ICollection<ValorSeleccion>> DescargarListaActividadEconomica();
    }
}