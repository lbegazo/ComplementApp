using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class ContratoRepository : IContratoRepository
    {
        private readonly DataContext _context;

        public ContratoRepository(DataContext context)
        {
            _context = context;
        }

        #region Contrato
        public async Task<Contrato> ObtenerContratoBase(int contratoId)
        {
            return await _context.Contrato
                        .FirstOrDefaultAsync(u => u.ContratoId == contratoId);
        }

        public async Task<ContratoDto> ObtenerContrato(int contratoId)
        {
            var lista = await (from t in _context.Contrato
                               join ti in _context.TipoModalidadContrato on t.TipoModalidadContratoId equals ti.TipoModalidadContratoId
                               where t.ContratoId == contratoId
                               select new ContratoDto()
                               {
                                   ContratoId = t.ContratoId,
                                   TipoModalidadContratoId = t.TipoModalidadContratoId,
                                   NumeroContrato = t.NumeroContrato,
                                   FechaInicio = (t.FechaInicio.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaInicio : null),
                                   FechaFinal = (t.FechaFinal.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaFinal : null),
                                   FechaExpedicionPoliza = t.FechaExpedicionPoliza.HasValue ? (t.FechaExpedicionPoliza.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaExpedicionPoliza : null) : null,
                                   Crp = t.Crp,
                                   Supervisor1Id = t.Supervisor1Id.HasValue ? t.Supervisor1Id.Value : null,
                                   Supervisor2Id = t.Supervisor2Id.HasValue ? t.Supervisor2Id.Value : null,
                               }).FirstOrDefaultAsync();

            return lista;
        }

        public async Task<PagedList<CDPDto>> ObtenerCompromisosSinContrato(int? terceroId, UserParams userParams)
        {
            var listaCompromisosConContrato = _context.Contrato.Select(x => x.Crp).ToHashSet();
            var listaCompromisosSinContrato = _context.CDP
                                                .Where(item => !listaCompromisosConContrato.Contains(item.Crp))
                                                .Select(x => x.Crp).ToHashSet();

            var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where listaCompromisosSinContrato.Contains(c.Crp)
                         where c.TerceroId == terceroId || terceroId == null
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Cdp = c.Cdp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             Objeto = c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                             SaldoActual = c.SaldoActual,
                             ValorTotal = c.ValorTotal,
                             TerceroId = c.TerceroId,

                         })
                       .OrderBy(t => t.Crp);

            var lista1 = (from i in lista
                          group i by new { i.Crp, i.Cdp, i.Detalle4, i.NumeroIdentificacionTercero, i.NombreTercero, i.Objeto, i.TerceroId }
                                      into grp
                          select new CDPDto()
                          {
                              Crp = grp.Key.Crp,
                              Cdp = grp.Key.Cdp,
                              Detalle4 = grp.Key.Detalle4,
                              Objeto = grp.Key.Objeto,
                              NumeroIdentificacionTercero = grp.Key.NumeroIdentificacionTercero,
                              NombreTercero = grp.Key.NombreTercero,
                              SaldoActual = grp.Sum(i => i.SaldoActual),
                              ValorTotal = grp.Sum(i => i.ValorTotal),
                              TerceroId = grp.Key.TerceroId,
                          })
                                      .OrderBy(x => x.Crp);

            return await PagedList<CDPDto>.CreateAsync(lista1, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CDPDto>> ObtenerCompromisosConContrato(int? terceroId, UserParams userParams)
        {
            var listaCompromisosConContrato = _context.Contrato.Select(x => x.Crp).ToHashSet();

            var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where listaCompromisosConContrato.Contains(c.Crp)
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Cdp = c.Cdp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             Objeto = c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                             SaldoActual = c.SaldoActual,
                             ValorTotal = c.ValorTotal,
                             TerceroId = c.TerceroId,
                         })
                        .OrderBy(x => x.Crp);

            var lista1 = (from i in lista
                          group i by new
                          {
                              i.Crp,
                              i.Cdp,
                              i.Detalle4,
                              i.NumeroIdentificacionTercero,
                              i.NombreTercero,
                              i.Objeto,
                              i.TerceroId
                          }
                          into grp
                          select new CDPDto()
                          {
                              Crp = grp.Key.Crp,
                              Cdp = grp.Key.Cdp,
                              Detalle4 = grp.Key.Detalle4,
                              Objeto = grp.Key.Objeto,
                              NumeroIdentificacionTercero = grp.Key.NumeroIdentificacionTercero,
                              NombreTercero = grp.Key.NombreTercero,
                              SaldoActual = grp.Sum(i => i.SaldoActual),
                              ValorTotal = grp.Sum(i => i.ValorTotal),
                              TerceroId = grp.Key.TerceroId,
                          })
                          .OrderBy(x => x.Crp);

            return await PagedList<CDPDto>.CreateAsync(lista1, userParams.PageNumber, userParams.PageSize);
        }

        #endregion Contrato

    }
}