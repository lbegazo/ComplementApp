using System.Collections.Generic;
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
                                   TipoContratoId = t.TipoContratoId,
                                   NumeroContrato = t.NumeroContrato,
                                   Crp = t.Crp,
                                   FechaInicio = t.FechaInicio,
                                   FechaFinal = t.FechaFinal,
                                   FechaExpedicionPoliza = t.FechaExpedicionPoliza,
                                   Supervisor1Id = t.Supervisor1Id,
                                   Supervisor2Id = t.Supervisor2Id.HasValue ? t.Supervisor2Id.Value : null,
                                   ValorPagoMensual = t.ValorPagoMensual,
                               }).FirstOrDefaultAsync();

            return lista;
        }

        public async Task<PagedList<CDPDto>> ObtenerCompromisosSinContrato(int? terceroId, UserParams userParams)
        {
            var listaCompromisosConContrato = (from c in _context.Contrato
                                               where c.PciId == userParams.PciId
                                               select c.Crp).ToHashSet();

             var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where !listaCompromisosConContrato.Contains(c.Crp)
                         where c.TerceroId == terceroId || terceroId == null
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0
                         where c.PciId == userParams.PciId
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
                          group i by new { i.Crp, i.Cdp, i.Detalle4, 
                                           i.NumeroIdentificacionTercero, 
                                           i.NombreTercero, i.Objeto, i.TerceroId }
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
            var lista = (from con in _context.Contrato
                         join c in _context.CDP on con.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         where con.PciId == userParams.PciId
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

        public async Task<ICollection<ContratoDto>> ObtenerListaContratoTotal(int pciId)
        {
            var lista = (from c in _context.Contrato
                         join tc in _context.TipoContrato on c.TipoContratoId equals tc.TipoContratoId
                         join sup1 in _context.Usuario on c.Supervisor1Id equals sup1.UsuarioId into Supervisor1
                         from super1 in Supervisor1.DefaultIfEmpty()
                         join sup2 in _context.Usuario on c.Supervisor2Id equals sup2.UsuarioId into Supervisor2
                         from super2 in Supervisor2.DefaultIfEmpty()
                         where c.PciId == pciId
                         select new ContratoDto()
                         {
                             NumeroContrato = c.NumeroContrato,
                             Crp = c.Crp,
                             TipoContrato = new ValorSeleccion()
                             {
                                 Nombre = tc.Nombre
                             },
                             FechaRegistro = c.FechaRegistro,
                             FechaInicio = c.FechaInicio,
                             FechaFinal = c.FechaFinal,
                             FechaExpedicionPoliza = c.FechaExpedicionPoliza,
                             Supervisor1 = new UsuarioParaDetalleDto()
                             {
                                 Nombres = super1.Nombres,
                                 Apellidos = super1.Apellidos,
                                 NombreCompleto = c.Supervisor1Id > 0 ? super1.Nombres + " " + super1.Apellidos : string.Empty,
                             },
                             Supervisor2 = new UsuarioParaDetalleDto()
                             {
                                 Nombres = super2.Nombres,
                                 Apellidos = super2.Apellidos,
                                 NombreCompleto = c.Supervisor2Id > 0 ? super2.Nombres + " " + super2.Apellidos : string.Empty,
                             },
                             ValorPagoMensual = c.ValorPagoMensual,
                         })
                        .Distinct()
                        .OrderBy(c => c.NumeroContrato);

            return await lista.ToListAsync();
        }
        #endregion Contrato

    }
}