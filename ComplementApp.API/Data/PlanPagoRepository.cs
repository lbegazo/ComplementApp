using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;

namespace ComplementApp.API.Data
{
    public class PlanPagoRepository : IPlanPagoRepository
    {
        private readonly DataContext _context;
        public PlanPagoRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlanPago>> ObtenerListaPlanPago(int terceroId, List<int> listaEstadoId)
        {
            var lista = await (from c in _context.PlanPago
                               join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                               where c.TerceroId == terceroId
                               where listaEstadoId.Contains(c.EstadoPlanPagoId.Value)
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
                                   TerceroId = c.TerceroId
                               })
                               .Distinct()
                               .ToListAsync();
            return lista;
        }

        public async Task<PlanPago> ObtenerPlanPago(int planPagoId)
        {
            return await _context.PlanPago.FirstOrDefaultAsync(u => u.PlanPagoId == planPagoId);
        }

        public async Task<DetallePlanPago> ObtenerDetallePlanPago(int planPagoId)
        {
            return await (from pp in _context.PlanPago
                          join c in _context.CDP on pp.Cdp equals c.Cdp
                          where pp.PlanPagoId == planPagoId
                          where c.Instancia == (int)TipoDocumento.Cdp
                          select new DetallePlanPago()
                          {
                              PlanPagoId = pp.PlanPagoId,
                              Detalle4 = c.Detalle4,
                              Detalle5 = c.Detalle5,
                              Detalle6 = c.Detalle6,
                              ValorTotal = c.ValorTotal,
                              SaldoActual = c.SaldoActual,
                              Fecha = c.Fecha
                          })                          
                    .FirstOrDefaultAsync();

                    /*
                          select c.Detalle6 + " - FECHA DE REGISTRO: " + c.Fecha.ToString("dd/MM/yyyy") +
                                  " - OBJETO: " + c.Detalle4 + 
                                  " - VALOR COMPROMISO: $" + c.ValorTotal.ToString("0.00") +
                                  " - SALDO ACTUAL COMPROMISO: $" +c.SaldoActual.ToString("0.00") +
                                  " - RESPONSABLE: " + c.Detalle5 )
                         */
        }
    }
}