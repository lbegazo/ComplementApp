using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface ICargaObligacionRepository
    {
        bool InsertarListaCargaObligacion(IList<CargaObligacion> listaCdp);
        bool EliminarCargaObligacion();
        Task<PagedList<CDPDto>> ObtenerListaCargaObligacion(string estado, UserParams userParams);
        Task<ICollection<CargaObligacionDto>> ObtenerListaCargaObligacionArchivoCabecera(int usuarioId, string estado, int pciId);
        Task<ICollection<CargaObligacionDto>> ObtenerListaCargaObligacionArchivoDetalle(string estado, int pciId);
    }
}