using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IActividadGeneralRepository
    {
        Task<ActividadGeneral> ObtenerActividadGeneralBase(int id);
        Task<ICollection<ActividadGeneral>> ObtenerActividadesGenerales(int pciId);
        Task<ICollection<ActividadEspecifica>> ObtenerActividadesEspecificas(int pciId);
        Task<ActividadEspecifica> ObtenerActividadEspecificaBase(int id);
    }
}