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
        public CDPRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CDP>> ObtenerListaCDP(int usuarioId)
        {

            var cdps = await (from c in _context.CDP
                              join d in _context.DetalleCDP on c.Cdp equals d.Cdp
                              where d.UsuarioId == usuarioId
                              where c.Instancia == (int)TipoDocumento.Cdp
                              group c by new
                              {
                                  c.Cdp,
                                  c.Fecha
                              } into g
                              select new CDP
                              {
                                  Cdp = g.Key.Cdp,
                                  Fecha = g.Key.Fecha
                              }).Distinct()
                              .ToListAsync();
            return cdps;
        }

        public async Task<PagedList<CDP>> ObtenerListaCompromiso(UserParams userParams)
        {
            var listaCompromisos = (from pp in _context.DetalleCDP
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

        public async Task<CDPDto> ObtenerCDP(int usuarioId, int numeroCDP)
        {
            var cdp = await (from c in _context.CDP
                             join d in _context.DetalleCDP on c.Cdp equals d.Cdp
                             where d.UsuarioId == usuarioId
                             where c.Cdp == numeroCDP
                             where c.Instancia == (int)TipoDocumento.Cdp
                             select new CDPDto()
                             {
                                 Instancia = c.Instancia,
                                 Cdp = c.Cdp,
                                 Fecha = c.Fecha,
                                 Detalle1 = c.Detalle1, //Estado
                                 Detalle4 = c.Detalle4 //Objeto del bien
                             }).FirstOrDefaultAsync();
            return cdp;
        }

        public async Task<IEnumerable<DetalleCDPDto>> ObtenerDetalleDeCDP(int usuarioId, int numeroCDP)
        {
            if (numeroCDP > 0)
            {
                return await ObtenerRubrosPresupuestalesConCDP(usuarioId, numeroCDP);
            }
            else
            {
                return await ObtenerRubrosPresupuestalesSinCDP(usuarioId, numeroCDP);
            }
        }

        public async Task<SolicitudCDPDto> ObtenerSolicitudCDP(int solicitudCDPId)
        {
            var solicitudCDP = await (from s in _context.SolicitudCDP
                                      join u in _context.Usuario on s.UsuarioIdRegistro equals u.UsuarioId
                                      join c in _context.Cargo on u.CargoId equals c.CargoId
                                      join a in _context.Area on u.AreaId equals a.AreaId
                                      join to in _context.TipoOperacion on s.TipoOperacionId equals to.TipoOperacionId
                                      join td in _context.TipoDetalleModificacion on s.TipoDetalleCDPId equals td.TipoDetalleCDPId into TipoDetalleCDP
                                      from td in TipoDetalleCDP.DefaultIfEmpty()
                                      where (s.SolicitudCDPId == solicitudCDPId)
                                      select new SolicitudCDPDto()
                                      {
                                          SolicitudCDPId = s.SolicitudCDPId,
                                          FechaSolicitud = s.FechaRegistro,
                                          FechaRegistro = s.FechaRegistro,
                                          Cdp = s.Cdp,
                                          EstadoCDP = s.EstadoCDP,
                                          NumeroActividad = s.NumeroActividad,
                                          AplicaContrato = s.AplicaContrato,
                                          AplicaContratoDescripcion = (s.AplicaContrato) ? ("SI") : ("NO"),
                                          NombreBienServicio = s.NombreBienServicio,
                                          ProyectoInversion = s.ProyectoInversion,
                                          ActividadProyectoInversion = s.ActividadProyectoInversion,
                                          ObjetoBienServicioContratado = s.ObjetoBienServicioContratado,
                                          Observaciones = s.Observaciones,
                                          TipoOperacionId = s.TipoOperacionId,
                                          TipoOperacion = new TipoOperacion()
                                          {
                                              TipoOperacionId = s.TipoOperacionId,
                                              Nombre = to.Nombre,
                                          },
                                          TipoDetalleCDPId = s.TipoDetalleCDPId.HasValue ? s.TipoDetalleCDPId.Value : 0,
                                          TipoDetalleCDP = new TipoDetalleCDP()
                                          {
                                              TipoDetalleCDPId = s.TipoDetalleCDPId.HasValue ? s.TipoDetalleCDPId.Value : 0,
                                              Nombre = s.TipoDetalleCDPId.HasValue ? td.Nombre : string.Empty,
                                          },
                                          Usuario = new UsuarioParaDetalleDto()
                                          {
                                              UsuarioId = u.UsuarioId,
                                              Nombres = u.Nombres,
                                              Apellidos = u.Apellidos,
                                              NombreCompleto = u.Nombres + " " + u.Apellidos,
                                              CargoId = c.CargoId,
                                              CargoNombre = c.Nombre,
                                              AreaId = a.AreaId,
                                              AreaNombre = a.Nombre,
                                          }

                                      }).FirstOrDefaultAsync();


            return solicitudCDP;
        }

        public async Task<PagedList<SolicitudCDPParaPrincipalDto>> ObtenerListaSolicitudCDP(int? solicitudId, int? tipoOperacionId,
                                                                                int? usuarioId, DateTime? fechaRegistro, int? estadoSolicitudId,
                                                                                UserParams userParams)
        {
            var listaSolicitudCDP = (from s in _context.SolicitudCDP
                                     join u in _context.Usuario on s.UsuarioIdRegistro equals u.UsuarioId
                                     join to in _context.TipoOperacion on s.TipoOperacionId equals to.TipoOperacionId
                                     join e in _context.EstadoSolicitudCDP on s.EstadoSolicitudCDPId equals e.EstadoId
                                     where (s.SolicitudCDPId == solicitudId || solicitudId == null)
                                     where (s.TipoOperacionId == tipoOperacionId || tipoOperacionId == null)
                                     where (s.UsuarioId == usuarioId || usuarioId == null)
                                     where (s.FechaRegistro.Date == fechaRegistro || fechaRegistro == null)
                                     where (s.EstadoSolicitudCDPId == estadoSolicitudId || estadoSolicitudId == null)
                                     select new SolicitudCDPParaPrincipalDto()
                                     {
                                         SolicitudCDPId = s.SolicitudCDPId,
                                         FechaRegistro = s.FechaRegistro,
                                         ObjetoBienServicioContratado = s.ObjetoBienServicioContratado.Length > 100 ? s.ObjetoBienServicioContratado.Substring(0, 100) + "..." : s.ObjetoBienServicioContratado,
                                         TipoOperacion = new TipoOperacion()
                                         {
                                             TipoOperacionId = s.TipoOperacionId,
                                             Nombre = to.Nombre,
                                         },
                                         Usuario = new UsuarioParaDetalleDto()
                                         {
                                             Nombres = u.Nombres,
                                             Apellidos = u.Apellidos,
                                             UsuarioId = u.UsuarioId,
                                             NombreCompleto = u.Nombres + " " + u.Apellidos,
                                         },
                                         EstadoSolicitudCDP = new ValorSeleccion()
                                         {
                                             Id = s.EstadoSolicitudCDPId,
                                             Nombre = e.Nombre,
                                         }
                                     }).OrderBy(s => s.SolicitudCDPId);


            return await PagedList<SolicitudCDPParaPrincipalDto>.CreateAsync(listaSolicitudCDP, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<ICollection<DetalleSolicitudCDP>> ObtenerDetalleSolicitudCDP(int solicitudCDPId)
        {
            var lista = await (from s in _context.SolicitudCDP
                               join d in _context.DetalleSolicitudCDP on s.SolicitudCDPId equals d.SolicitudCDPId
                               join r in _context.RubroPresupuestal on d.RubroPresupuestalId equals r.RubroPresupuestalId
                               where s.SolicitudCDPId == solicitudCDPId
                               select new DetalleSolicitudCDP()
                               {
                                   DetalleSolicitudCDPId = d.DetalleSolicitudCDPId,
                                   RubroPresupuestalId = r.RubroPresupuestalId,
                                   RubroPresupuestal = new RubroPresupuestal()
                                   {
                                       Identificacion = r.Identificacion,
                                       Nombre = r.Nombre,
                                   },
                                   SaldoActividad = d.SaldoActividad,
                                   SaldoCDP = d.SaldoCDP,
                                   ValorCDP = d.ValorCDP,
                                   ValorActividad = d.ValorActividad,
                                   ValorSolicitud = d.ValorSolicitud
                               }
                                        ).ToListAsync();

            return lista;
        }

        public async Task<ICollection<DetalleCDP>> ObtenerRubrosPresupuestalesPorCompromiso(long crp, int pciId)
        {
            var detalles = await (from d in _context.CDP
                                  join i in _context.RubroPresupuestal on d.RubroPresupuestalId equals i.RubroPresupuestalId
                                  join sf in _context.SituacionFondo on d.Detalle9.ToUpper() equals sf.Nombre.ToUpper()
                                  join ff in _context.FuenteFinanciacion on d.Detalle8.ToUpper() equals ff.Nombre.ToUpper()
                                  join rp in _context.RecursoPresupuestal on d.Detalle10.ToUpper() equals rp.Codigo.ToUpper()
                                  join cp in _context.ClavePresupuestalContable on d.Crp equals cp.Crp
                                  where d.Instancia == (int)TipoDocumento.Compromiso
                                  where d.PciId == pciId
                                  where d.Crp == crp
                                  where d.Detalle2 == cp.Dependencia
                                  where d.RubroPresupuestalId == cp.RubroPresupuestalId
                                  where sf.SituacionFondoId == cp.SituacionFondoId
                                  where ff.FuenteFinanciacionId == cp.FuenteFinanciacionId
                                  where rp.RecursoPresupuestalId == cp.RecursoPresupuestalId
                                  where d.SaldoActual > 0
                                  select new DetalleCDP()
                                  {
                                      ValorCDP = d.ValorInicial,
                                      ValorOP = d.Operacion,
                                      ValorTotal = d.ValorTotal,
                                      SaldoAct = d.SaldoActual,
                                      Dependencia = d.Detalle2,
                                      DependenciaDescripcion = d.Detalle2 + " " + (d.Detalle3.Length > 100 ? d.Detalle3.Substring(0, 100) + "..." : d.Detalle3),
                                      ClavePresupuestalContableId = cp.ClavePresupuestalContableId,
                                      RubroPresupuestal = new RubroPresupuestal()
                                      {
                                          RubroPresupuestalId = i.RubroPresupuestalId,
                                          Identificacion = i.Identificacion,
                                          Nombre = i.Nombre,
                                      }
                                  })
                                 .Distinct()
                                 .OrderBy(x => x.RubroPresupuestal.Identificacion)
                                 .ToListAsync();
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

        private async Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesConCDP(int usuarioId, int numeroCDP)
        {
            var detalles = await (from d in _context.DetalleCDP
                                  join i in _context.RubroPresupuestal on d.RubroPresupuestalId equals i.RubroPresupuestalId
                                  join dec in _context.RubroPresupuestal on d.DecretoId equals dec.RubroPresupuestalId
                                  join c in _context.CDP on d.Cdp equals c.Cdp
                                  join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                                  join ag in _context.ActividadGeneral on new { d.ActividadGeneralId } equals new { ag.ActividadGeneralId } into ActividadGeneralDetalle
                                  from acGe in ActividadGeneralDetalle.DefaultIfEmpty()
                                  join rp in _context.RubroPresupuestal on acGe.RubroPresupuestalId equals rp.RubroPresupuestalId into RubroActividadGeneral
                                  from ruAG in RubroActividadGeneral.DefaultIfEmpty()
                                  join ae in _context.ActividadEspecifica on new { d.ActividadEspecificaId } equals new { ae.ActividadEspecificaId } into ActividadEspecificaDetalle
                                  from acEs in ActividadEspecificaDetalle.DefaultIfEmpty()
                                  join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                                  from de in DependenciaDetalle.DefaultIfEmpty()
                                  join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                                  from a in AreaDetalle.DefaultIfEmpty()
                                  where c.PciId == acGe.PciId
                                  where c.PciId == acEs.PciId
                                  where c.PciId == u.PciId
                                  where d.RubroPresupuestalId == c.RubroPresupuestalId
                                  where c.Instancia == (int)TipoDocumento.Cdp
                                  where d.UsuarioId == usuarioId
                                  where d.Cdp == numeroCDP
                                  select new DetalleCDPDto()
                                  {
                                      DetalleCdpId = d.DetalleCdpId,
                                      PcpId = d.PcpId,
                                      IdArchivo = d.IdArchivo,
                                      Cdp = d.Cdp,
                                      Proy = d.Proy,
                                      Prod = d.Prod,
                                      Proyecto = ruAG.Nombre,
                                      ActividadBpin = acEs.Nombre,
                                      PlanDeCompras = d.PlanDeCompras,
                                      Responsable = u.Nombres + ' ' + u.Apellidos,
                                      Dependencia = de.Nombre,
                                      ValorCDP = c.ValorTotal,
                                      SaldoCDP = c.SaldoActual,
                                      ValorAct = d.ValorAct,
                                      SaldoAct = d.SaldoAct,
                                      ValorRP = d.ValorRP,
                                      ValorOB = d.ValorOB,
                                      ValorOP = d.ValorOP,
                                      AplicaContratoDescripcion = d.AplicaContrato ? "SI" : "NO",
                                      SaldoTotal = d.SaldoTotal,
                                      SaldoDisponible = d.SaldoDisponible,
                                      Area = a.Nombre,
                                      Valor_Convenio = d.Valor_Convenio,
                                      Convenio = d.Convenio,
                                      Decreto = dec.Identificacion,
                                      RubroPresupuestal = new RubroPresupuestalDto()
                                      {
                                          RubroPresupuestalId = i.RubroPresupuestalId,
                                          Identificacion = i.Identificacion,
                                          Nombre = i.Nombre,
                                      }
                                  })
                                 .Distinct()
                                 .OrderBy(x => x.RubroPresupuestal.Identificacion)
                                 .ToListAsync();

            return detalles;
        }

        //Para solicitud inicial no tiene CDP asociado
        private async Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesSinCDP(int usuarioId, int numeroCDP)
        {
            var detalles = await (from d in _context.DetalleCDP
                                  join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                                  join r in _context.RubroPresupuestal on d.RubroPresupuestalId equals r.RubroPresupuestalId
                                  join dec in _context.RubroPresupuestal on d.DecretoId equals dec.RubroPresupuestalId
                                  join ag in _context.ActividadGeneral on new { d.ActividadGeneralId } equals new { ag.ActividadGeneralId } into ActividadGeneralDetalle
                                  from acGe in ActividadGeneralDetalle.DefaultIfEmpty()
                                  join rp in _context.RubroPresupuestal on acGe.RubroPresupuestalId equals rp.RubroPresupuestalId into RubroActividadGeneral
                                  from rpAG in RubroActividadGeneral.DefaultIfEmpty()
                                  join ae in _context.ActividadEspecifica on new { d.ActividadEspecificaId } equals new { ae.ActividadEspecificaId } into ActividadEspecificaDetalle
                                  from acEs in ActividadEspecificaDetalle.DefaultIfEmpty()
                                  join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                                  from de in DependenciaDetalle.DefaultIfEmpty()
                                  join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                                  from a in AreaDetalle.DefaultIfEmpty()
                                  where d.PciId == u.PciId
                                  where d.PciId == acGe.PciId
                                  where d.PciId == acEs.PciId
                                  where d.PciId == a.PciId
                                  where d.UsuarioId == usuarioId
                                  where d.Cdp == numeroCDP

                                  select new DetalleCDPDto()
                                  {
                                      DetalleCdpId = d.DetalleCdpId,
                                      PcpId = d.PcpId,
                                      IdArchivo = d.IdArchivo,
                                      Cdp = d.Cdp,
                                      Proy = d.Proy,
                                      Prod = d.Prod,
                                      Proyecto = rpAG.Nombre,
                                      ActividadBpin = acEs.Nombre,
                                      PlanDeCompras = d.PlanDeCompras,
                                      Responsable = u.Nombres + " " + u.Apellidos,
                                      Dependencia = de.Nombre,
                                      ValorAct = d.ValorAct,
                                      SaldoAct = d.SaldoAct,
                                      ValorRP = d.ValorRP,
                                      ValorOB = d.ValorOB,
                                      ValorOP = d.ValorOP,
                                      AplicaContratoDescripcion = d.AplicaContrato ? "SI" : "NO",
                                      SaldoTotal = d.SaldoTotal,
                                      SaldoDisponible = d.SaldoDisponible,
                                      Crp = d.Crp,
                                      Area = a.Nombre,
                                      Valor_Convenio = d.Valor_Convenio,
                                      Convenio = d.Convenio,
                                      Decreto = dec.Identificacion,
                                      RubroPresupuestal = new RubroPresupuestalDto()
                                      {
                                          RubroPresupuestalId = r.RubroPresupuestalId,
                                          Identificacion = r.Identificacion,
                                          Nombre = r.Nombre,
                                      }
                                  })
                                 .Distinct()
                                 .OrderBy(x => x.RubroPresupuestal.Identificacion)
                                 .ToListAsync()
                                 ;
            return detalles;
        }

    }
}