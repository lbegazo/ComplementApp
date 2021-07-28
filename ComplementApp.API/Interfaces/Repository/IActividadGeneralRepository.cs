using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IActividadGeneralRepository
    {
        Task<ActividadGeneral> ObtenerActividadGeneralBase(int id);
        Task<ICollection<ActividadGeneralDto>> ObtenerActividadesGenerales(int pciId);
        Task<ICollection<ActividadEspecifica>> ObtenerActividadesEspecificas(int pciId);
        Task<PagedList<ActividadEspecifica>> ObtenerListaActividadEspecifica(UserParams userParams);
        Task<ActividadEspecifica> ObtenerActividadEspecificaBase(int id);
    }
}