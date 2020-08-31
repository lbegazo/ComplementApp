using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

            // var documento = await (from c in _context.PlanPago
            //                        where c.PlanPagoId == planPagoId)
            //                        .FirstOrDefaultAsync();
            //    select new PlanPago()
            //    {
            //        PlanPagoId = c.PlanPagoId,
            //        Cdp = c.Cdp,
            //        Crp = c.Crp,
            //        AnioPago = c.AnioPago,
            //        MesPago = c.MesPago,
            //        ValorAPagar = c.ValorAPagar,
            //        Viaticos = c.Viaticos,
            //        TerceroId = c.TerceroId,

            //        NumeroPago = c.NumeroPago,
            //        NumeroRadicadoProveedor = c.NumeroRadicadoProveedor,
            //        FechaRadicadoProveedor = c.FechaRadicadoProveedor,
            //        NumeroFactura = c.NumeroFactura,
            //        ValorFacturado = c.ValorFacturado,
            //        Observaciones = c.Observaciones

            //    }).FirstOrDefaultAsync();
            //return documento;
        }
    }
}