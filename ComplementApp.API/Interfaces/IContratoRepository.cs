using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Interfaces
{
    public interface IContratoRepository
    {
        Task<ContratoDto> ObtenerContrato(int contratoId);
        Task<PagedList<CDPDto>> ObtenerCompromisosSinContrato(int? terceroId, UserParams userParams);
        Task<PagedList<CDPDto>> ObtenerCompromisosConContrato(int? terceroId, UserParams userParams);
    };
}