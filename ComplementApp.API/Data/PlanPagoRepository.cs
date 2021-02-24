using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;
using Microsoft.Data.SqlClient;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using AutoMapper;
using System.Globalization;

namespace ComplementApp.API.Data
{
    public class PlanPagoRepository : IPlanPagoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;

        public PlanPagoRepository(DataContext context, IMapper mapper, IGeneralInterface generalInterface)
        {
            _mapper = mapper;
            _context = context;
            this._generalInterface = generalInterface;
        }

        public async Task<PagedList<PlanPago>> ObtenerListaPlanPago(int? terceroId, List<int> listaEstadoId, UserParams userParams)
        {
            var lista = (from c in _context.PlanPago
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where (c.TerceroId == terceroId || terceroId == null)
                         where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         select new PlanPago()
                         {
                             PlanPagoId = c.PlanPagoId,
                             Cdp = c.Cdp,
                             Crp = c.Crp,
                             AnioPago = c.AnioPago,
                             MesPago = c.MesPago,
                             ValorAPagar = c.ValorAPagar,
                             Viaticos = c.Viaticos,
                             NumeroPago = c.NumeroPago,
                             EstadoPlanPagoId = c.EstadoPlanPagoId,
                             NumeroRadicadoSupervisor = c.NumeroRadicadoSupervisor,
                             FechaRadicadoSupervisor = c.FechaRadicadoSupervisor,
                             ValorFacturado = c.ValorFacturado,
                             TerceroId = c.TerceroId,
                             Tercero = new Tercero()
                             {
                                 TerceroId = c.TerceroId,
                                 NumeroIdentificacion = t.NumeroIdentificacion,
                                 Nombre = t.Nombre,
                                 ModalidadContrato = pl.ModalidadContrato,
                                 TipoPago = pl.TipoPago
                             }
                         })
                         .Distinct()
                        .OrderBy(c => c.FechaRadicadoSupervisor);

            return await PagedList<PlanPago>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<List<PlanPago>> ObtenerListaPlanPagoXIds(List<int> listaPlanPagoId)
        {
            return await (from c in _context.PlanPago
                          join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                          join t in _context.Tercero on c.TerceroId equals t.TerceroId
                          join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                          from pl in parametroLiquidacion.DefaultIfEmpty()
                          where (listaPlanPagoId.Contains(c.PlanPagoId))
                          select new PlanPago()
                          {
                              PlanPagoId = c.PlanPagoId,
                              Cdp = c.Cdp,
                              Crp = c.Crp,
                              AnioPago = c.AnioPago,
                              MesPago = c.MesPago,
                              ValorAPagar = c.ValorAPagar,
                              Viaticos = c.Viaticos,
                              NumeroPago = c.NumeroPago,
                              EstadoPlanPagoId = c.EstadoPlanPagoId,
                              NumeroRadicadoSupervisor = c.NumeroRadicadoSupervisor,
                              FechaRadicadoSupervisor = c.FechaRadicadoSupervisor,
                              ValorFacturado = c.ValorFacturado,
                              TerceroId = c.TerceroId,
                              Tercero = new Tercero()
                              {
                                  TerceroId = c.TerceroId,
                                  NumeroIdentificacion = t.NumeroIdentificacion,
                                  Nombre = t.Nombre,
                                  ModalidadContrato = pl.ModalidadContrato,
                                  TipoPago = pl.TipoPago,
                                  TipoIva = pl.TipoIva.HasValue ? pl.TipoIva.Value : 0,
                              }
                          })
                        .OrderBy(c => c.FechaRadicadoSupervisor)
                        .ToListAsync();
        }

        public async Task<PagedList<PlanPago>> ObtenerListaPlanPagoXCompromiso(long crp, List<int> listaEstadoId, UserParams userParams)
        {
            var lista = (from c in _context.PlanPago
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where (c.Crp == crp)
                         where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         select new PlanPago()
                         {
                             PlanPagoId = c.PlanPagoId,
                             Cdp = c.Cdp,
                             Crp = c.Crp,
                             AnioPago = c.AnioPago,
                             MesPago = c.MesPago,
                             ValorAPagar = c.ValorAPagar,
                             Viaticos = c.Viaticos,
                             NumeroPago = c.NumeroPago,
                             EstadoPlanPagoId = c.EstadoPlanPagoId,
                             NumeroRadicadoSupervisor = c.NumeroRadicadoSupervisor,
                             FechaRadicadoSupervisor = c.FechaRadicadoSupervisor,
                             ValorFacturado = c.ValorFacturado,
                             TerceroId = c.TerceroId,
                             Tercero = new Tercero()
                             {
                                 TerceroId = c.TerceroId,
                                 NumeroIdentificacion = t.NumeroIdentificacion,
                                 Nombre = t.Nombre,
                                 ModalidadContrato = pl.ModalidadContrato,
                                 TipoPago = pl.TipoPago
                             }
                         })
                               .OrderBy(c => c.FechaRadicadoSupervisor);

            return await PagedList<PlanPago>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PlanPago> ObtenerPlanPagoBase(int planPagoId)
        {
            return await _context.PlanPago.FirstOrDefaultAsync(u => u.PlanPagoId == planPagoId);
        }

        //Utilizado para mostrar la cabecera de la liquidación
        public async Task<DetallePlanPagoDto> ObtenerDetallePlanPago(int planPagoId)
        {
            return await (from pp in _context.PlanPago
                          join c in _context.CDP on pp.Crp equals c.Crp
                          join t in _context.Tercero on pp.TerceroId equals t.TerceroId
                          join sp in _context.FormatoSolicitudPago on pp.PlanPagoId equals sp.PlanPagoId into SolicitudPago
                          from sol in SolicitudPago.DefaultIfEmpty()

                          join p in _context.ParametroLiquidacionTercero on pp.TerceroId equals p.TerceroId into ParametroTercero
                          from pt in ParametroTercero.DefaultIfEmpty()

                          join us in _context.Usuario on pp.UsuarioIdRegistro equals us.UsuarioId into Usuario
                          from us in Usuario.DefaultIfEmpty()

                          join con in _context.Contrato on pp.Crp equals con.Crp into Contrato
                          from contra in Contrato.DefaultIfEmpty()
                          join sup in _context.Usuario on contra.Supervisor1Id equals sup.UsuarioId into Supervisor
                          from super in Supervisor.DefaultIfEmpty()

                          where pp.PlanPagoId == planPagoId
                          where c.Instancia == (int)TipoDocumento.Compromiso
                          where sol.EstadoId == (int)EstadoSolicitudPago.Aprobado
                          select new DetallePlanPagoDto()
                          {
                              PlanPagoId = pp.PlanPagoId,
                              TerceroId = pp.TerceroId,
                              Detalle4 = CortarTexto(c.Detalle4, 50),
                              Detalle5 = super.Nombres + ' ' + super.Apellidos,
                              Detalle6 = c.Detalle6,
                              Detalle7 = ResumirDetalle7(c.Detalle7),
                              ValorTotal = c.ValorTotal,
                              SaldoActual = c.SaldoActual,
                              Fecha = c.Fecha,
                              Operacion = c.Operacion,
                              ModalidadContrato = pt.ModalidadContrato,
                              TipoPago = pt.TipoPago,

                              Viaticos = pp.Viaticos,
                              ViaticosDescripcion = pp.Viaticos ? "SI" : "NO",
                              Crp = pp.Crp,
                              NumeroPago = pp.NumeroPago,
                              ValorFacturado = pp.ValorFacturado.HasValue ? pp.ValorFacturado.Value : 0,
                              NumeroRadicadoSupervisor = pp.NumeroRadicadoSupervisor,
                              FechaRadicadoSupervisor = pp.FechaRadicadoSupervisor,
                              FechaRadicadoSupervisorFormato = pp.FechaRadicadoSupervisor.HasValue ? pp.FechaRadicadoSupervisor.Value.ToString("yyyy-MM-dd") : string.Empty,
                              NumeroFactura = sol.NumeroFactura,
                              Observaciones = sol.ObservacionesModificacion.Length > 100 ? sol.ObservacionesModificacion.Substring(0, 100) : sol.ObservacionesModificacion,
                              NumeroRadicadoProveedor = pp.NumeroRadicadoProveedor,
                              FechaRadicadoProveedor = pp.FechaRadicadoProveedor.HasValue ? pp.FechaRadicadoProveedor.Value : null,
                              FechaRadicadoProveedorFormato = pp.FechaRadicadoProveedor.HasValue ? pp.FechaRadicadoProveedor.Value.ToString("yyyy-MM-dd") : string.Empty,

                              IdentificacionTercero = t.NumeroIdentificacion,
                              NombreTercero = CortarTexto(t.Nombre, 30),

                              Usuario = us.Nombres + ' ' + us.Apellidos,
                              Email = us.Email,
                          })
                    .FirstOrDefaultAsync();
        }

        //Utilizado para mostrar la cabecera de la Solicitud Pago
        public async Task<DetallePlanPagoDto> ObtenerDetallePlanPagoParaSolicitudPago(int planPagoId)
        {
            return await (from pp in _context.PlanPago
                          join c in _context.CDP on pp.Crp equals c.Crp
                          join t in _context.Tercero on pp.TerceroId equals t.TerceroId

                          join p in _context.ParametroLiquidacionTercero on pp.TerceroId equals p.TerceroId into ParametroTercero
                          from pt in ParametroTercero.DefaultIfEmpty()

                          join us in _context.Usuario on pp.UsuarioIdRegistro equals us.UsuarioId into Usuario
                          from us in Usuario.DefaultIfEmpty()


                          join con in _context.Contrato on pp.Crp equals con.Crp into Contrato
                          from contra in Contrato.DefaultIfEmpty()
                          join sup in _context.Usuario on contra.Supervisor1Id equals sup.UsuarioId into Supervisor
                          from super in Supervisor.DefaultIfEmpty()


                          where pp.PlanPagoId == planPagoId
                          where c.Instancia == (int)TipoDocumento.Compromiso
                          select new DetallePlanPagoDto()
                          {
                              PlanPagoId = pp.PlanPagoId,
                              TerceroId = pp.TerceroId,
                              Detalle4 = CortarTexto(c.Detalle4, 50),
                              Detalle5 = super.Nombres + ' ' + super.Apellidos,
                              Detalle6 = c.Detalle6,
                              Detalle7 = ResumirDetalle7(c.Detalle7),
                              ValorTotal = c.ValorTotal,
                              SaldoActual = c.SaldoActual,
                              Fecha = c.Fecha,
                              Operacion = c.Operacion,
                              ModalidadContrato = pt.ModalidadContrato,
                              TipoPago = pt.TipoPago,

                              Viaticos = pp.Viaticos,
                              ViaticosDescripcion = pp.Viaticos ? "SI" : "NO",
                              Crp = pp.Crp,
                              NumeroPago = pp.NumeroPago,
                              ValorFacturado = pp.ValorFacturado.HasValue ? pp.ValorFacturado.Value : 0,
                              NumeroRadicadoSupervisor = pp.NumeroRadicadoSupervisor,
                              FechaRadicadoSupervisor = pp.FechaRadicadoSupervisor,
                              FechaRadicadoSupervisorFormato = pp.FechaRadicadoSupervisor.HasValue ? pp.FechaRadicadoSupervisor.Value.ToString("yyyy-MM-dd") : string.Empty,
                              NumeroRadicadoProveedor = pp.NumeroRadicadoProveedor,
                              FechaRadicadoProveedor = pp.FechaRadicadoProveedor.HasValue ? pp.FechaRadicadoProveedor.Value : null,
                              FechaRadicadoProveedorFormato = pp.FechaRadicadoProveedor.HasValue ? pp.FechaRadicadoProveedor.Value.ToString("yyyy-MM-dd") : string.Empty,

                              IdentificacionTercero = t.NumeroIdentificacion,
                              NombreTercero = CortarTexto(t.Nombre, 30),

                              Usuario = us.Nombres + ' ' + us.Apellidos,
                              Email = us.Email,
                          })
                    .FirstOrDefaultAsync();
        }

        public async Task<ICollection<DetallePlanPagoDto>> ObtenerListaDetallePlanPagoXIds(List<int> listaPlanPagoId)
        {
            return await (from pp in _context.PlanPago
                          join c in _context.CDP on pp.Crp equals c.Crp
                          join t in _context.Tercero on pp.TerceroId equals t.TerceroId
                          join sp in _context.FormatoSolicitudPago on pp.PlanPagoId equals sp.PlanPagoId

                          join p in _context.ParametroLiquidacionTercero on pp.TerceroId equals p.TerceroId into ParametroTercero
                          from pt in ParametroTercero.DefaultIfEmpty()

                          join u in _context.Usuario on pp.UsuarioIdRegistro equals u.UsuarioId into Usuario
                          from us in Usuario.DefaultIfEmpty()

                          join con in _context.Contrato on pp.Crp equals con.Crp into Contrato
                          from contra in Contrato.DefaultIfEmpty()
                          join sup in _context.Usuario on contra.Supervisor1Id equals sup.UsuarioId into Supervisor
                          from super in Supervisor.DefaultIfEmpty()

                          where listaPlanPagoId.Contains(pp.PlanPagoId)
                          where c.Instancia == (int)TipoDocumento.Compromiso
                          select new DetallePlanPagoDto()
                          {
                              PlanPagoId = pp.PlanPagoId,
                              TerceroId = pp.TerceroId,
                              Detalle4 = CortarTexto(c.Detalle4, 50),
                              Detalle5 = super.Nombres + ' ' + super.Apellidos,
                              Detalle6 = c.Detalle6,
                              Detalle7 = ResumirDetalle7(c.Detalle7),
                              ValorTotal = c.ValorTotal,
                              SaldoActual = c.SaldoActual,
                              Fecha = c.Fecha,
                              Operacion = c.Operacion,
                              ModalidadContrato = pt.ModalidadContrato,
                              TipoPago = pt.TipoPago,

                              Viaticos = pp.Viaticos,
                              ViaticosDescripcion = pp.Viaticos ? "SI" : "NO",
                              Crp = pp.Crp,
                              NumeroPago = pp.NumeroPago,
                              ValorFacturado = pp.ValorFacturado.HasValue ? pp.ValorFacturado.Value : 0,
                              NumeroRadicadoSupervisor = pp.NumeroRadicadoSupervisor,
                              FechaRadicadoSupervisor = pp.FechaRadicadoSupervisor,
                              FechaRadicadoSupervisorFormato = pp.FechaRadicadoSupervisor.HasValue ? pp.FechaRadicadoSupervisor.Value.ToString("yyyy-MM-dd") : string.Empty,
                              NumeroFactura = pp.NumeroFactura,
                              Observaciones = pp.Observaciones,
                              NumeroRadicadoProveedor = pp.NumeroRadicadoProveedor,
                              FechaRadicadoProveedor = pp.FechaRadicadoProveedor.Value,

                              IdentificacionTercero = t.NumeroIdentificacion,
                              NombreTercero = CortarTexto(t.Nombre, 30),

                              Usuario = us.Nombres + ' ' + us.Apellidos,
                              Email = us.Email,
                              TextoComprobanteContable = c.Detalle6 +
                                " FACTURA: " +
                                sp.NumeroFactura +
                                " OBSERVACIONES: " +
                                (sp.ObservacionesModificacion.Length > 100 ? sp.ObservacionesModificacion.Substring(0, 100) : sp.ObservacionesModificacion) +
                                " RAD_PROV: " +
                                pp.NumeroRadicadoProveedor +
                                " FECHA: " +
                                (pp.FechaRadicadoProveedor.HasValue ? pp.FechaRadicadoProveedor.Value.ToString("yyyy-MM-dd") : string.Empty) +
                                " RAD_SUP: " +
                                pp.NumeroRadicadoSupervisor +
                                " FECHA: " +
                                (pp.FechaRadicadoSupervisor.HasValue ? pp.FechaRadicadoSupervisor.Value.ToString("yyyy-MM-dd") : string.Empty) +
                                " APRUEBA: " +
                                super.Nombres + ' ' + super.Apellidos
                          })
                    .ToListAsync();
        }

        public async Task<PlanPago> ObtenerPlanPagoDetallado(int planPagoId)
        {
            PlanPago planPago = new PlanPago();
            var parameter = new SqlParameter("planPagoId", planPagoId);

            var lista = await _context.PlanPago
                                        .FromSqlRaw("EXECUTE dbo.USP_PlanPago_ObtenerDetallado @planPagoId", parameter)
                                        .ToListAsync();

            if (lista != null)
            {
                planPago = lista[0];
            }

            return planPago;
        }

        public int ObtenerCantidadMaximaPlanPago(long crp)
        {
            var cantidad = _context.PlanPago.Where(pp => pp.Crp == crp).Max(x => x.NumeroPago);
            return cantidad;
        }

        public async Task<ICollection<DetallePlanPagoDto>> ObtenerListaCantidadMaximaPlanPago(List<long> compromisos)
        {
            var query = await (from t in _context.PlanPago
                               where compromisos.Contains(t.Crp)
                               group t by new { t.Crp }
                         into grp
                               select new DetallePlanPagoDto()
                               {
                                   Crp = grp.Key.Crp,
                                   CantidadPago = grp.Max(x => x.NumeroPago),
                               }).ToListAsync();

            return query;
        }

        public async Task RegistrarPlanPago(PlanPago plan)
        {
            await _context.PlanPago.AddAsync(plan);
            //bool resultado = await _unitOfWork.CompleteAsync();
            //return resultado;
        }

        public void ActualizarPlanPago(PlanPago plan)
        {
            _context.PlanPago.Update(plan);
        }

        public async Task<ICollection<RadicadoDto>> ObtenerListaRadicado(int mes, int? terceroId, List<int> listaEstadoId)
        {
            int anio = _generalInterface.ObtenerFechaHoraActual().Year;
            var lista = (from c in _context.PlanPago
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join d in _context.DetalleLiquidacion on c.PlanPagoId equals d.PlanPagoId into liquidacion
                         from li in liquidacion.DefaultIfEmpty()
                         where (c.FechaRadicadoSupervisor.Value.Month == mes)
                         where (c.FechaRadicadoSupervisor.Value.Year == anio)
                         where (c.TerceroId == terceroId || terceroId == null)
                         where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         select new RadicadoDto()
                         {
                             PlanPagoId = c.PlanPagoId,
                             FechaRadicadoSupervisor = c.FechaRadicadoSupervisor.Value,
                             FechaRadicadoSupervisorDescripcion = c.FechaRadicadoSupervisor.Value.ToString("yyyy-MM-dd"),
                             Estado = e.Descripcion,
                             Crp = c.Crp.ToString(),
                             NumeroRadicadoProveedor = c.NumeroRadicadoProveedor,
                             NumeroRadicadoSupervisor = c.NumeroRadicadoSupervisor,
                             ValorAPagar = c.ValorFacturado.Value,
                             NIT = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                             Obligacion = li.Obligacion.HasValue ? li.Obligacion.Value.ToString() : string.Empty,
                             OrdenPago = li.OrdenPago.HasValue ? li.OrdenPago.Value.ToString() : string.Empty,
                             FechaOrdenPago = li.FechaOrdenPago,
                             FechaOrdenPagoDescripcion = li.FechaOrdenPago.HasValue ? li.FechaOrdenPago.Value.ToString("yyyy-MM-dd") : string.Empty,
                             TextoComprobanteContable = li.TextoComprobanteContable != null ? li.TextoComprobanteContable : string.Empty,
                         })
                        .OrderBy(c => c.FechaRadicadoSupervisor)
                        .ThenBy(c => c.Crp);

            return await lista.ToListAsync();
        }
        public async Task<PagedList<RadicadoDto>> ObtenerListaRadicadoPaginado(int mes, int? terceroId, List<int> listaEstadoId, UserParams userParams)
        {
            var lista = (from c in _context.PlanPago
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join d in _context.DetalleLiquidacion on c.PlanPagoId equals d.PlanPagoId into liquidacion
                         from li in liquidacion.DefaultIfEmpty()
                         where (c.FechaRadicadoSupervisor.Value.Month == mes)
                         where (c.TerceroId == terceroId || terceroId == null)
                         where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         select new RadicadoDto()
                         {
                             PlanPagoId = c.PlanPagoId,
                             FechaRadicadoSupervisor = c.FechaRadicadoSupervisor.Value,
                             FechaRadicadoSupervisorDescripcion = c.FechaRadicadoSupervisor.Value.ToString("yyyy-MM-dd"),
                             Estado = e.Descripcion,
                             Crp = c.Crp.ToString(),
                             NumeroRadicadoProveedor = c.NumeroRadicadoProveedor,
                             NumeroRadicadoSupervisor = c.NumeroRadicadoSupervisor,
                             ValorAPagar = c.ValorAPagar,
                             NIT = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                             Obligacion = li.Obligacion.HasValue ? li.OrdenPago.Value.ToString() : string.Empty,
                             OrdenPago = li.OrdenPago.HasValue ? li.OrdenPago.Value.ToString() : string.Empty,
                             FechaOrdenPago = li.FechaOrdenPago,
                             FechaOrdenPagoDescripcion = li.FechaOrdenPago.HasValue ? li.FechaOrdenPago.Value.ToString("yyyy-MM-dd") : string.Empty,
                             TextoComprobanteContable = li.TextoComprobanteContable != null ? li.TextoComprobanteContable : string.Empty,
                         })
                        .OrderBy(c => c.FechaRadicadoSupervisor)
                        .ThenBy(c => c.Crp);

            return await PagedList<RadicadoDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize); ;
        }


        #region Forma Pago Compromiso

        public async Task<PagedList<CDPDto>> ObtenerCompromisosSinPlanPago(int? terceroId, int? numeroCrp, UserParams userParams)
        {
            var listaCompromisos = _context.PlanPago.Select(x => x.Crp).ToHashSet();
            var notFoundItems = _context.CDP.Where(item => !listaCompromisos.Contains(item.Crp)).Select(x => x.Crp).ToHashSet();

            var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where notFoundItems.Contains(c.Crp)
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.Crp == numeroCrp || numeroCrp == null
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

        public async Task<PagedList<CDPDto>> ObtenerCompromisosConPlanPago(int? terceroId, int? numeroCrp, UserParams userParams)
        {
            var listaCompromisos = _context.PlanPago.Select(x => x.Crp).ToHashSet();

            var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where listaCompromisos.Contains(c.Crp)
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.Crp == numeroCrp || numeroCrp == null
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

        public async Task<ICollection<LineaPlanPagoDto>> ObtenerLineasPlanPagoXCompromiso(int numeroCrp)
        {

            var lista = await (from pp in _context.PlanPago
                               join c in _context.CDP on pp.Crp equals c.Crp
                               where pp.TerceroId == c.TerceroId
                               where c.Instancia == (int)TipoDocumento.Compromiso
                               where c.SaldoActual > 0 //Saldo Disponible
                               where c.Crp == numeroCrp
                               select new LineaPlanPagoDto()
                               {
                                   PlanPagoId = pp.PlanPagoId,
                                   MesId = pp.MesPago,
                                   Valor = pp.ValorInicial,
                                   Viaticos = pp.Viaticos,

                               })
                         .Distinct()
                         .ToListAsync();

            foreach (var item in lista)
            {
                item.MesDescripcion = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.MesId).ToUpper();
            }

            var lista1 = lista
                        .OrderBy(x => x.MesId)
                        .ToList();

            return lista1;
        }

        public async Task<PlanPago> ObtenerUltimoPlanPagoDeCompromisoXMes(int crp, int MesId)
        {
            PlanPago ultimoPlanPago = new PlanPago();
            var lista = await (from c in _context.PlanPago
                               where (c.Crp == crp)
                               where (c.MesPago == MesId)
                               select new PlanPago()
                               {
                                   PlanPagoId = c.PlanPagoId,
                                   Cdp = c.Cdp,
                                   Crp = c.Crp,
                                   AnioPago = c.AnioPago,
                                   MesPago = c.MesPago,
                                   ValorAPagar = c.ValorAPagar,
                                   Viaticos = c.Viaticos,
                                   NumeroPago = c.NumeroPago,
                                   EstadoPlanPagoId = c.EstadoPlanPagoId,
                                   NumeroRadicadoSupervisor = c.NumeroRadicadoSupervisor,
                                   FechaRadicadoSupervisor = c.FechaRadicadoSupervisor,
                                   ValorFacturado = c.ValorFacturado,
                                   TerceroId = c.TerceroId
                               }).ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                ultimoPlanPago = lista.OrderBy(x => x.NumeroPago).Last();
            }

            return ultimoPlanPago;
        }

        #endregion Forma Pago Compromiso
        private static string ResumirDetalle7(string texto)
        {
            string resultado = string.Empty;
            switch (texto)
            {
                case "'CONTRATO DE PRESTACION DE SERVICIOS - PROFESIONALES":
                    resultado = "CPSP";
                    break;
                case "CONTRATO DE PRESTACION DE SERVICIOS":
                    resultado = "CPS";
                    break;
                case "CONTRATO DE COMPRA VENTA Y SUMINISTROS":
                    resultado = "CCVS";
                    break;
                case "CONTRATO DE CONSULTORIA":
                    resultado = "CC";
                    break;
                case "CONTRATO DE OBRA":
                    resultado = "CO";
                    break;
                case "ACTO ADMINISTRATIVO":
                    resultado = "AA";
                    break;
                case "CONTRATO DE ARRENDAMIENTO":
                    resultado = "CA";
                    break;
                case "CONTRATO INTERADMINISTRATIVO":
                    resultado = "CIA";
                    break;
                case "ORDEN DE COMPRA":
                    resultado = "OC";
                    break;
                case "FACTURA":
                    resultado = "FACT";
                    break;
                case "CONVENIO":
                    resultado = "CONV";
                    break;
                case "RESOLUCION":
                    resultado = "RES";
                    break;
                case "RECIBOS OFICIALES DE PAGO":
                    resultado = "ROP";
                    break;
                default: break;
            }

            return resultado;
        }

        private static string CortarTexto(string texto, int longitud)
        {
            string resultado = string.Empty;
            resultado = texto.Length > longitud ? (texto.Substring(0, longitud)) : (texto);
            return resultado;
        }

    }
}