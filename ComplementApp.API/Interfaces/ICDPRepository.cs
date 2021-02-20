using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using System;

namespace ComplementApp.API.Interfaces
{
    public interface ICDPRepository
    {
        Task<IEnumerable<CDP>> ObtenerListaCDP(int usuarioId);

        Task<CDPDto> ObtenerCDP(int usuarioId, int numeroCDP);

        Task<IEnumerable<DetalleCDPDto>> ObtenerDetalleDeCDP(int usuarioId, int numeroCDP);

        Task<SolicitudCDPDto> ObtenerSolicitudCDP(int solicitudCDPId);

        Task<ICollection<DetalleSolicitudCDP>> ObtenerDetalleSolicitudCDP(int solicitudCDPId);

        Task<PagedList<SolicitudCDPParaPrincipalDto>> ObtenerListaSolicitudCDP(int? solicitudId, int? tipoOperacion,
                                                                    int? usuarioId, DateTime? fechaRegistro,
                                                                    int? estadoSolicitudId, UserParams userParams);
        Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesPorCompromiso(long crp);

        Task<CDPDto> ObtenerCDPPorCompromiso(long crp);
    }
}