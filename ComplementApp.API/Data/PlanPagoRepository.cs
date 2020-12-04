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
                               .OrderBy(c => c.FechaRadicadoSupervisor);

            return await PagedList<PlanPago>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize); ;
        }


        public async Task<PlanPago> ObtenerPlanPagoBase(int planPagoId)
        {
            return await _context.PlanPago.FirstOrDefaultAsync(u => u.PlanPagoId == planPagoId);
        }

        public async Task<DetallePlanPagoDto> ObtenerDetallePlanPago(int planPagoId)
        {
            return await (from pp in _context.PlanPago
                          join c in _context.CDP on pp.Crp equals c.Crp
                          join t in _context.Tercero on pp.TerceroId equals t.TerceroId
                          join r in _context.RubroPresupuestal on pp.RubroPresupuestalId equals r.RubroPresupuestalId

                          join p in _context.ParametroLiquidacionTercero on pp.TerceroId equals p.TerceroId into ParametroTercero
                          from pt in ParametroTercero.DefaultIfEmpty()

                          join u in _context.UsoPresupuestal on pp.UsoPresupuestalId equals u.UsoPresupuestalId into UsosPresupuestales
                          from up in UsosPresupuestales.DefaultIfEmpty()

                          join u in _context.Usuario on pp.UsuarioIdRegistro equals u.UsuarioId into Usuario
                          from us in Usuario.DefaultIfEmpty()

                          where pp.PlanPagoId == planPagoId
                          where c.Instancia == (int)TipoDocumento.Compromiso
                          select new DetallePlanPagoDto()
                          {
                              PlanPagoId = pp.PlanPagoId,
                              TerceroId = pp.TerceroId,
                              Detalle4 = CortarTexto(c.Detalle4, 50),
                              Detalle5 = c.Detalle5,
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

                              IdentificacionRubroPresupuestal = r.Identificacion,
                              IdentificacionUsoPresupuestal = up.Identificacion,
                              IdentificacionTercero = t.NumeroIdentificacion,
                              NombreTercero = CortarTexto(t.Nombre, 30),

                              Usuario = us.Nombres + ' ' + us.Apellidos,
                              Email = us.Email,
                          })
                    .FirstOrDefaultAsync();


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

        public async Task<ICollection<Deduccion>> ObtenerDeduccionesXTercero(int terceroId, int? actividadEconomicaId)
        {
            var query = (from d in _context.Deduccion
                         join td in _context.TerceroDeducciones on d.DeduccionId equals td.DeduccionId
                         where (td.TerceroId == terceroId)
                         where (td.ActividadEconomicaId == actividadEconomicaId || actividadEconomicaId == null)
                         where (d.estado == true)
                         select d);

            return await query.ToListAsync();
        }

        public int ObtenerCantidadMaximaPlanPago(long crp)
        {
            var cantidad = _context.PlanPago.Where(pp => pp.Crp == crp).Max(x => x.NumeroPago);
            return cantidad;
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
                             ValorAPagar = c.ValorAPagar,
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