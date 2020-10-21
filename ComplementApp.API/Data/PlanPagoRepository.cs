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

namespace ComplementApp.API.Data
{
    public class PlanPagoRepository : IPlanPagoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public PlanPagoRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
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

        public async Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerListaDetalleLiquidacion(int? terceroId, List<int> listaEstadoId, UserParams userParams)
        {
            var lista = (from dl in _context.DetalleLiquidacion
                         join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where (c.TerceroId == terceroId || terceroId == null)
                         where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         select new FormatoCausacionyLiquidacionPagos()
                         {
                             DetalleLiquidacionId = dl.DetalleLiquidacionId,
                             PlanPagoId = dl.PlanPagoId,
                             IdentificacionTercero = dl.NumeroIdentificacion,
                             NombreTercero = dl.Nombre,
                             NumeroRadicadoSupervisor = dl.NumeroRadicado,
                             FechaRadicadoSupervisor = dl.FechaRadicado,
                             ValorTotal = c.ValorFacturado.Value
                         })
                               .OrderBy(c => c.FechaRadicadoSupervisor);

            return await PagedList<FormatoCausacionyLiquidacionPagos>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize); ;
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

                              //   join dc in _context.DetalleCDP on pp.Cdp equals dc.Cdp into DetalleCDP
                              //   from dcdp in DetalleCDP.DefaultIfEmpty()

                          join u in _context.Usuario on pp.UsuarioIdRegistro equals u.UsuarioId into Usuario
                          from us in Usuario.DefaultIfEmpty()

                          where pp.PlanPagoId == planPagoId
                          where c.Instancia == (int)TipoDocumento.Compromiso
                          select new DetallePlanPagoDto()
                          {
                              PlanPagoId = pp.PlanPagoId,
                              TerceroId = pp.TerceroId,
                              Detalle4 = c.Detalle4,
                              Detalle5 = c.Detalle5,
                              Detalle6 = c.Detalle6,
                              Detalle7 = c.Detalle7,
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
                              NumeroFactura = pp.NumeroFactura,
                              Observaciones = pp.Observaciones,
                              NumeroRadicadoProveedor = pp.NumeroRadicadoProveedor,
                              FechaRadicadoProveedor = pp.FechaRadicadoProveedor.Value,

                              IdentificacionRubroPresupuestal = r.Identificacion,
                              IdentificacionUsoPresupuestal = up.Identificacion,
                              IdentificacionTercero = t.NumeroIdentificacion,
                              NombreTercero = t.Nombre,
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

        public async Task<ICollection<Deduccion>> ObtenerDeduccionesXTercero(int terceroId)
        {
            var query = (from d in _context.Deduccion
                         join td in _context.TerceroDeducciones on d.DeduccionId equals td.DeduccionId
                         where (td.TerceroId == terceroId)
                         where (d.estado == true)
                         select d);

            return await query.ToListAsync();
        }

        public int ObtenerCantidadMaximaPlanPago(long crp)
        {
            var cantidad = _context.PlanPago.Where(pp => pp.Crp == crp).Max(x => x.NumeroPago);
            return cantidad;
        }

        public async Task<FormatoCausacionyLiquidacionPagos> ObtenerDetalleFormatoCausacionyLiquidacionPago(long detalleLiquidacionId)
        {
            DeduccionDto deduccionDto = null;

            var detalleLiquidacion = await (from dl in _context.DetalleLiquidacion
                                            join pp in _context.PlanPago on dl.PlanPagoId equals pp.PlanPagoId
                                            where dl.DetalleLiquidacionId == detalleLiquidacionId
                                            select new FormatoCausacionyLiquidacionPagos()
                                            {
                                                //Plan de pago
                                                DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                                PlanPagoId = dl.PlanPagoId,
                                                TerceroId = pp.TerceroId,
                                                ModalidadContrato = dl.ModalidadContrato,
                                                IdentificacionTercero = dl.NumeroIdentificacion,
                                                NombreTercero = dl.Nombre,
                                                Contrato = dl.Contrato,
                                                Viaticos = pp.Viaticos,
                                                ViaticosDescripcion = dl.Viaticos,
                                                Crp = dl.Crp.ToString(),
                                                CantidadPago = dl.CantidadPago,
                                                NumeroPago = dl.NumeroPago,

                                                ValorContrato = dl.ValorContrato,
                                                ValorAdicionReduccion = dl.ValorAdicionReduccion,
                                                ValorCancelado = dl.ValorCancelado,
                                                TotalACancelar = dl.TotalACancelar,
                                                SaldoActual = dl.SaldoActual,
                                                IdentificacionRubroPresupuestal = dl.RubroPresupuestal,
                                                IdentificacionUsoPresupuestal = dl.UsoPresupuestal,

                                                NombreSupervisor = dl.NombreSupervisor,
                                                NumeroRadicadoSupervisor = dl.NumeroRadicado,
                                                FechaRadicadoSupervisor = dl.FechaRadicado,
                                                NumeroFactura = dl.NumeroFactura,

                                                TextoComprobanteContable = dl.TextoComprobanteContable,

                                                //Formato de Liquidación
                                                Honorario = dl.Honorario,
                                                HonorarioUvt = (int)dl.HonorarioUvt,
                                                ValorIva = dl.ValorIva,
                                                ValorTotal = dl.ValorTotal,
                                                TotalRetenciones = dl.TotalRetenciones,
                                                TotalAGirar = dl.TotalAGirar,

                                                BaseSalud = dl.BaseSalud,
                                                AporteSalud = (int)dl.AporteSalud,
                                                AportePension = dl.AportePension,
                                                RiesgoLaboral = dl.RiesgoLaboral,
                                                FondoSolidaridad = dl.FondoSolidaridad,
                                                ImpuestoCovid = dl.ImpuestoCovid,
                                                SubTotal1 = dl.SubTotal1,

                                                PensionVoluntaria = dl.PensionVoluntaria,
                                                Afc = (int)dl.Afc,
                                                SubTotal2 = dl.SubTotal2,
                                                MedicinaPrepagada = dl.MedicinaPrepagada,
                                                Dependientes = dl.Dependientes,
                                                InteresVivienda = dl.InteresesVivienda,
                                                TotalDeducciones = dl.TotalDeducciones,

                                                SubTotal3 = dl.SubTotal3,
                                                RentaExenta = (int)dl.RentaExenta,
                                                LimiteRentaExenta = dl.LimiteRentaExenta,
                                                TotalRentaExenta = dl.TotalRentaExenta,
                                                DiferencialRenta = dl.DiferencialRenta,
                                                BaseGravableRenta = dl.BaseGravableRenta,
                                                BaseGravableUvt = (int)dl.BaseGravableUvt,
                                                ViaticosPagados = dl.ViaticosPagados,
                                            }).FirstOrDefaultAsync();

            if (detalleLiquidacion != null)
            {
                #region Setear datos

                //Deducciones
                var deduccionesLiquidacion = await _context.LiquidacionDeducciones
                                                .Where(x => x.DetalleLiquidacionId == detalleLiquidacion.DetalleLiquidacionId)
                                                .ToListAsync();
                if (deduccionesLiquidacion != null && deduccionesLiquidacion.Count > 0)
                {
                    detalleLiquidacion.Deducciones = new List<DeduccionDto>();
                    foreach (var deduccion in deduccionesLiquidacion)
                    {
                        deduccionDto = new DeduccionDto();
                        deduccionDto.Codigo = deduccion.Codigo;
                        deduccionDto.Nombre = deduccion.Nombre;
                        deduccionDto.Base = deduccion.Base;
                        deduccionDto.Tarifa = deduccion.Tarifa;
                        deduccionDto.Valor = deduccion.Valor;
                        detalleLiquidacion.Deducciones.Add(deduccionDto);
                    }
                }

                #endregion Setear datos
            }

            return detalleLiquidacion;
        }

        public async Task<DetalleLiquidacion> ObtenerDetalleLiquidacionBase(int detalleLiquidacion)
        {
            return await _context.DetalleLiquidacion.FirstOrDefaultAsync(u => u.DetalleLiquidacionId == detalleLiquidacion);
        }

        public async Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionAnterior(long terceroId)
        {
            int mesAnterior = System.DateTime.Now.AddMonths(-1).Month;
            var detalleLiquidacionAnterior = await (from dl in _context.DetalleLiquidacion
                                                    join pp in _context.PlanPago on dl.PlanPagoId equals pp.PlanPagoId
                                                    where pp.TerceroId == terceroId
                                                    where dl.FechaRegistro.Value.Month == mesAnterior
                                                    select dl)
                                            .ToListAsync();

            return detalleLiquidacionAnterior;
        }

        public async Task RegistrarDetalleLiquidacion(DetalleLiquidacion detalleLiquidacion)
        {
            await _context.DetalleLiquidacion.AddAsync(detalleLiquidacion);
            //bool resultado = await _unitOfWork.CompleteAsync();
            //return resultado;
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

    }
}