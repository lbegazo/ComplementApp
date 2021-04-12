using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Service
{
    public interface IActividadGeneralService
    {
        Task ActualizarListaActividadGeneral(int pciId, List<ActividadGeneral> listaTotal);
        Task ActualizarActividadEspecifica(int pciId, ActividadEspecifica actividadEspecifica);
        Task ActualizarActividadGeneral(ActividadGeneral actividadGeneral, decimal valor, int operacion);
        Task ActualizarActividadEspecifica(ActividadEspecifica actividad, decimal valor, int operacion);
    }
}