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
                               where t.ContratoId == contratoId
                               select new ContratoDto()
                               {
                                   ContratoId = t.ContratoId,
                                   TipoModalidadContratoId = t.TipoModalidadContratoId,
                                   NumeroContrato = t.NumeroContrato,
                                   Crp = t.Crp,
                                   FechaInicio = t.FechaInicio,
                                   FechaFinal = t.FechaFinal,
                                   FechaExpedicionPoliza = t.FechaExpedicionPoliza,
                                   Supervisor1Id = t.Supervisor1Id,
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
            var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join con in _context.Contrato on c.Crp equals con.Crp
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
                             ContratoId = con.ContratoId,
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
                              i.TerceroId,
                              i.ContratoId,
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
                              ContratoId = grp.Key.ContratoId,
                          })
                          .OrderBy(x => x.Crp);

            return await PagedList<CDPDto>.CreateAsync(lista1, userParams.PageNumber, userParams.PageSize);
        }

        #endregion Contrato

    }
}