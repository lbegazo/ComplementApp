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
    public class SolicitudCdpRepository : ISolicitudCdpRepository
    {
        private readonly DataContext _context;
        public SolicitudCdpRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<CDPDto> ObtenerCDP(int usuarioId, int numeroCDP)
        {
            var cdp = await (from c in _context.CDP
                             join d in _context.PlanAdquisicion on c.Cdp equals d.Cdp
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

        public async Task<IEnumerable<CDP>> ObtenerListaCDP(int usuarioId)
        {

            var cdps = await (from c in _context.CDP
                              join d in _context.PlanAdquisicion on c.Cdp equals d.Cdp
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
    }
}