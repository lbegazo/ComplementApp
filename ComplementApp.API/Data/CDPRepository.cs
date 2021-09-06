using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Helpers;
using System;

namespace ComplementApp.API.Data
{
    public class CDPRepository : ICDPRepository
    {
        private readonly DataContext _context;
        private readonly IGeneralInterface _generalInterface;
        public CDPRepository(DataContext context, IGeneralInterface generalInterface)
        {
            _context = context;
            _generalInterface = generalInterface;
        }

        public async Task<PagedList<CDP>> ObtenerListaCompromiso(UserParams userParams)
        {
            var listaCompromisos = (from pp in _context.PlanAdquisicion
                                    where pp.PciId == userParams.PciId
                                    select pp.Crp).ToHashSet();

            var lista = (from c in _context.CDP
                         where c.PciId == userParams.PciId
                         where !listaCompromisos.Contains(c.Crp)
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         select new CDP()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4,
                             SaldoActual = c.SaldoActual,
                         })
                         .Distinct()
                         .OrderBy(c => c.Crp);

            return await PagedList<CDP>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesPorCompromiso(long crp, int pciId)
        {
            var detalles = await (from d in _context.CDP
                                  join i in _context.RubroPresupuestal on d.RubroPresupuestalId equals i.RubroPresupuestalId
                                  join sf in _context.SituacionFondo on d.Detalle9.ToUpper() equals sf.Nombre.ToUpper()
                                  join ff in _context.FuenteFinanciacion on d.Detalle8.ToUpper() equals ff.Nombre.ToUpper()
                                  join rp in _context.RecursoPresupuestal on d.Detalle10.ToUpper() equals rp.Codigo.ToUpper()
                                  join cp in _context.ClavePresupuestalContable on new
                                  {
                                      d.Crp,
                                      d.RubroPresupuestalId,
                                      sf.SituacionFondoId,
                                      ff.FuenteFinanciacionId,
                                      rp.RecursoPresupuestalId,
                                      Dependencia = d.Detalle2
                                  } equals
                                    new
                                    {
                                        cp.Crp,
                                        cp.RubroPresupuestalId,
                                        cp.SituacionFondoId,
                                        cp.FuenteFinanciacionId,
                                        cp.RecursoPresupuestalId,
                                        Dependencia = cp.Dependencia
                                    } into ClavePresupuestal
                                  from clave in ClavePresupuestal.DefaultIfEmpty()
                                  where d.Instancia == (int)TipoDocumento.Compromiso
                                  where d.PciId == pciId
                                  where d.Crp == crp
                                  where d.SaldoActual > 0
                                  select new DetalleCDPDto()
                                  {
                                      ValorCDP = d.ValorInicial,
                                      ValorOP = d.Operacion,
                                      ValorTotal = d.ValorTotal,
                                      SaldoAct = d.SaldoActual,
                                      Dependencia = d.Detalle2,
                                      DependenciaDescripcion = d.Detalle2 + " " + (d.Detalle3.Length > 100 ? d.Detalle3.Substring(0, 100) + "..." : d.Detalle3),
                                      ClavePresupuestalContableId = clave.ClavePresupuestalContableId,
                                      RubroPresupuestal = new RubroPresupuestal()
                                      {
                                          RubroPresupuestalId = i.RubroPresupuestalId,
                                          Identificacion = i.Identificacion,
                                          Nombre = i.Nombre,
                                      },
                                      CdpDocumento = new CDP()
                                      {
                                          Detalle8 = d.Detalle8,
                                          Detalle9 = d.Detalle9,
                                          Detalle10 = d.Detalle10,
                                      }
                                  })
                                 .Distinct()
                                 .OrderBy(x => x.RubroPresupuestal.Identificacion)
                                 .ToListAsync();
            return detalles;
        }

        public async Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesXNumeroContrato(string numeroContrato)
        {
            var detalles = await (from d in _context.CDP
                                  join i in _context.RubroPresupuestal on d.RubroPresupuestalId equals i.RubroPresupuestalId
                                  join pci in _context.Pci on d.PciId equals pci.PciId
                                  join sf in _context.SituacionFondo on d.Detalle9.ToUpper() equals sf.Nombre.ToUpper()
                                  join ff in _context.FuenteFinanciacion on d.Detalle8.ToUpper() equals ff.Nombre.ToUpper()
                                  join rp in _context.RecursoPresupuestal on d.Detalle10.ToUpper() equals rp.Codigo.ToUpper()
                                  join cp in _context.ClavePresupuestalContable on new
                                  {
                                      d.Crp,
                                      d.RubroPresupuestalId,
                                      sf.SituacionFondoId,
                                      ff.FuenteFinanciacionId,
                                      rp.RecursoPresupuestalId,
                                      Dependencia = d.Detalle2
                                  } equals
                                    new
                                    {
                                        cp.Crp,
                                        cp.RubroPresupuestalId,
                                        cp.SituacionFondoId,
                                        cp.FuenteFinanciacionId,
                                        cp.RecursoPresupuestalId,
                                        Dependencia = cp.Dependencia
                                    } into ClavePresupuestal
                                  from clave in ClavePresupuestal.DefaultIfEmpty()
                                  where d.Instancia == (int)TipoDocumento.Compromiso
                                  where d.Detalle6.Trim() == numeroContrato
                                  // where d.SaldoActual > 0
                                  select new DetalleCDPDto()
                                  {
                                      ValorCDP = d.ValorInicial,
                                      ValorOP = d.Operacion,
                                      ValorTotal = d.ValorTotal,
                                      SaldoAct = d.SaldoActual,
                                      Dependencia = d.Detalle2,
                                      DependenciaDescripcion = d.Detalle2 + " " + (d.Detalle3.Length > 100 ? d.Detalle3.Substring(0, 100) + "..." : d.Detalle3),
                                      ClavePresupuestalContableId = clave.ClavePresupuestalContableId,
                                      IdentificacionPci = pci.Identificacion,
                                      Crp = d.Crp,
                                      RubroPresupuestal = new RubroPresupuestal()
                                      {
                                          RubroPresupuestalId = i.RubroPresupuestalId,
                                          Identificacion = i.Identificacion,
                                          Nombre = i.Nombre,
                                      },
                                      CdpDocumento = new CDP()
                                      {
                                          Detalle8 = d.Detalle8,
                                          Detalle9 = d.Detalle9,
                                          Detalle10 = d.Detalle10,
                                      }
                                  })
                                 .Distinct()
                                 .OrderBy(x => x.RubroPresupuestal.Identificacion)
                                 .ToListAsync();

            #region Modificar nombre de rubro

            if (detalles != null)
            {
                string nombre = string.Empty;
                foreach (var item in detalles)
                {
                    if (item.RubroPresupuestal != null)
                    {
                        if (item.RubroPresupuestal.Identificacion.Contains("C-"))
                        {
                            string[] words = item.RubroPresupuestal.Nombre.Split('-');
                            if (words != null && words.Length > 1)
                            {
                                item.RubroPresupuestal.Nombre = _generalInterface.ObtenerCadenaLimitada(words[1], 40);
                            }
                        }
                        else
                        {
                            item.RubroPresupuestal.Nombre = _generalInterface.ObtenerCadenaLimitada(item.RubroPresupuestal.Nombre, 40);
                        }
                    }
                }
            }

            #endregion Modificar nombre de rubro

            return detalles;
        }

        public async Task<CDPDto> ObtenerCDPPorCompromiso(long crp)
        {
            var cdp = await (from d in _context.CDP
                             where d.Instancia == (int)TipoDocumento.Compromiso
                             where d.Crp == crp
                             select new CDPDto()
                             {
                                 Cdp = d.Cdp,
                                 Crp = d.Crp,
                                 Fecha = d.Fecha,
                             })
                            .Distinct()
                            .FirstOrDefaultAsync();
            return cdp;
        }

        public async Task<PagedList<CDPDto>> ObtenerDetallePlanAnualAdquisicion(long cdp, int instancia, UserParams userParams)
        {
            IQueryable<CDPDto> lista = Enumerable.Empty<CDPDto>().AsQueryable();

            if (instancia == 10)
            {
                int id = 0;
                var listaPrimaria = (from c in _context.PlanAdquisicionHistorico
                                     where c.PlanAdquisicioId == cdp
                                     where c.PciId == userParams.PciId
                                     select c.PlanAdquisicionHistoricoId)
                                    .ToList();

                if (listaPrimaria != null && listaPrimaria.Count > 0)
                {
                    id = listaPrimaria.Min();

                    lista = (from c in _context.PlanAdquisicionHistorico
                             join t in _context.Transaccion on c.TransaccionId equals t.TransaccionId
                             join u in _context.Usuario on c.UsuarioId equals u.UsuarioId
                             where c.PlanAdquisicioId == cdp
                             where c.PciId == userParams.PciId
                             where c.PlanAdquisicionHistoricoId != id
                             select new CDPDto()
                             {
                                 CdpId = c.PlanAdquisicioId,
                                 Cdp = c.PlanAdquisicionHistoricoId,
                                 Fecha = c.FechaRegistro.Value,
                                 Operacion = c.Valor,
                                 Detalle1 = t.Nombre,
                                 Detalle2 = u.Nombres + " " + u.Apellidos,
                             })
                       .Distinct();
                }
                else
                {
                    lista = (from c in _context.PlanAdquisicionHistorico
                             join t in _context.Transaccion on c.TransaccionId equals t.TransaccionId
                             join u in _context.Usuario on c.UsuarioId equals u.UsuarioId
                             where c.PlanAdquisicioId == cdp
                             where c.PciId == userParams.PciId
                             select new CDPDto()
                             {
                                 CdpId = c.PlanAdquisicioId,
                                 Cdp = c.PlanAdquisicionHistoricoId,
                                 Fecha = c.FechaRegistro.Value,
                                 Operacion = c.Valor,
                                 Detalle1 = t.Nombre,
                                 Detalle2 = u.Nombres + " " + u.Apellidos,
                             })
                      .Distinct()
                      .AsQueryable();
                }
            }
            else
            {
                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId into TerceroCdp
                         from terceroCdp in TerceroCdp.DefaultIfEmpty()
                         where c.Cdp == cdp
                         where c.PciId == userParams.PciId
                         where c.Instancia == instancia
                         select new CDPDto()
                         {
                             Fecha = c.Fecha,
                             FechaFormato = c.Fecha.ToString(),
                             Cdp = c.Cdp,
                             Crp = c.Crp,
                             Obligacion = c.Obligacion,
                             OrdenPago = c.OrdenPago,
                             ValorInicial = c.ValorInicial,
                             Operacion = c.Operacion,
                             ValorTotal = c.ValorTotal,
                             SaldoActual = c.SaldoActual,
                             Detalle1 = c.Detalle1,
                             Detalle4 = c.Detalle4,
                             NumeroIdentificacionTercero = c.TerceroId > 0 ? terceroCdp.NumeroIdentificacion : string.Empty,
                             NombreTercero = c.TerceroId > 0 ? terceroCdp.Nombre : string.Empty,
                             NumeroDocumento = (instancia == (int)TipoDocumento.Cdp ? c.Cdp :
                                                (instancia == (int)TipoDocumento.Compromiso ? c.Crp :
                                                (instancia == (int)TipoDocumento.Obligacion ? c.Obligacion :
                                                (instancia == (int)TipoDocumento.OrdenPago ? c.OrdenPago : 0))))
                         })
                            .Distinct()
                            .AsQueryable();
            }
            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CDPDto>> ObtenerListaCdpParaVinculacion(long? cdp, int instancia, UserParams userParams)
        {
            var listaSolicitud = (from pp in _context.SolicitudCDP
                                  where pp.PciId == userParams.PciId
                                  where pp.Cdp != null
                                  select pp.Cdp).ToHashSet();

            var lista = (from c in _context.CDP
                         join rp in _context.RubroPresupuestal on c.RubroPresupuestalId equals rp.RubroPresupuestalId
                         where c.PciId == userParams.PciId
                         where c.Instancia == instancia
                         where !listaSolicitud.Contains(c.Cdp)
                         where c.Cdp == cdp || cdp == null
                         select new CDPDto()
                         {
                             Cdp = c.Cdp,
                             ValorTotal = c.ValorTotal,
                             SaldoActual = c.SaldoActual,
                             Detalle4 = c.Detalle4,
                             IdentificacionRubro = rp.Identificacion,
                             NombreRubro = rp.Nombre
                         })
                         .OrderBy(x => x.Cdp);

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }
    }
}