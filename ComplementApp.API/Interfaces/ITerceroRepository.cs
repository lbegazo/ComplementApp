using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface ITerceroRepository
    {
         Task<PagedList<TerceroDto>> ObtenerTercerosParaParametrizacionLiquidacion(int tipo, int? terceroId, UserParams userParams);

         Task<ParametroLiquidacionTerceroDto> ObtenerParametrizacionLiquidacionXTercero(int terceroId);

         Task<ParametroLiquidacionTercero> ObtenerParametrizacionLiquidacionTerceroBase(int parametroLiquidacionTerceroId);
    }
}