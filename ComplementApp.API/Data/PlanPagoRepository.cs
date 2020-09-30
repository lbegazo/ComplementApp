using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;
using Microsoft.Data.SqlClient;
using ComplementApp.API.Helpers;

namespace ComplementApp.API.Data
{
    public class PlanPagoRepository : IPlanPagoRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public PlanPagoRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<PagedList<PlanPago>> ObtenerListaPlanPago(int? terceroId, List<int> listaEstadoId, UserParams userParams)
        {
            var lista = (from c in _context.PlanPago
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId
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
                                 ModalidadContrato = p.ModalidadContrato,
                                 TipoPago = p.TipoPago
                             }
                         })
                               .OrderBy(c => c.FechaRadicadoSupervisor);

            return await PagedList<PlanPago>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize); ;
        }

        public async Task<PlanPago> ObtenerPlanPago(int planPagoId)
        {
            return await _context.PlanPago.FirstOrDefaultAsync(u => u.PlanPagoId == planPagoId);
        }

        public async Task<DetallePlanPagoDto> ObtenerDetallePlanPago(int planPagoId)
        {
            return await (from pp in _context.PlanPago
                          join c in _context.CDP on pp.Crp equals c.Crp
                          join t in _context.Tercero on pp.TerceroId equals t.TerceroId
                          join p in _context.ParametroLiquidacionTercero on pp.TerceroId equals p.TerceroId
                          join r in _context.RubroPresupuestal on pp.RubroPresupuestalId equals r.RubroPresupuestalId
                          join u in _context.UsoPresupuestal on pp.UsoPresupuestalId equals u.UsoPresupuestalId
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
                              ModalidadContrato = p.ModalidadContrato,
                              TipoPago = p.TipoPago,

                              ViaticosDescripcion = pp.Viaticos ? "SI" : "NO",
                              Crp = pp.Crp,
                              NumeroPago = pp.NumeroPago,
                              ValorFacturado = pp.ValorFacturado.HasValue ? pp.ValorFacturado.Value : 0,
                              NumeroRadicadoSupervisor = pp.NumeroRadicadoSupervisor,
                              FechaRadicadoSupervisor = pp.FechaRadicadoSupervisor,
                              NumeroFactura = pp.NumeroFactura,
                              Observaciones = pp.Observaciones,

                              IdentificacionRubroPresupuestal = r.Identificacion,
                              IdentificacionUsoPresupuestal = u.Identificacion,
                              IdentificacionTercero = t.NumeroIdentificacion,
                              NombreTercero = t.Nombre

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

        public async Task<bool> RegistrarDetalleLiquidacion(DetalleLiquidacion detalleLiquidacion)
        {
            await _context.DetalleLiquidacion.AddAsync(detalleLiquidacion);
            bool resultado = await _unitOfWork.CompleteAsync();
            return resultado;
        }

        public int ObtenerCantidadMaximaPlanPago(long crp)
        {
            var cantidad = _context.PlanPago.Where(pp => pp.Crp == crp).Max(x => x.NumeroPago);
            return cantidad;
        }
    }
}