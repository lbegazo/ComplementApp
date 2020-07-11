using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class CDPRepository : ICDPRepository
    {
        private readonly DataContext _context;
        public CDPRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CDP>> ObtenerListaCDP(string usuario)
        {

            var cdps = await (from c in _context.CDP
                              join d in _context.DetalleCDP on c.Cdp equals d.Cdp
                              where d.Responsable == usuario
                              group c by new
                              {
                                  c.Cdp,
                                  c.Estado,
                                  c.Fecha
                              } into g
                              select new CDP
                              {
                                  Cdp = g.Key.Cdp,
                                  Estado = g.Key.Estado,
                                  Fecha = g.Key.Fecha
                              }).ToListAsync();
            return cdps;
        }

        public async Task<CDP> ObtenerCDP(string usuario, int numeroCDP)
        {
            var cdp = await (from c in _context.CDP
                             join d in _context.DetalleCDP on c.Cdp equals d.Cdp
                             where d.Responsable == usuario
                             where c.Cdp == numeroCDP
                             select c).FirstOrDefaultAsync();

            // var rubrosPresupuestales = await ObtenerItemsPresupuestalesDeCdp(usuario, numeroCDP);
            // cdp.RubrosPresupuestales = rubrosPresupuestales;

            return cdp;
        }


        public async Task<IEnumerable<DetalleCDP>> ObtenerDetalleDeCDP(string usuario, int numeroCDP)
        {
            return await ObtenerItemsPresupuestalesDeCdp(usuario, numeroCDP);
        }

        private async Task<ICollection<DetalleCDP>> ObtenerItemsPresupuestalesDeCdp(string usuario, int numeroCDP)
        {
            var detalles = await (from d in _context.DetalleCDP
                                 where d.Responsable == usuario
                                 where d.Cdp == numeroCDP
                                 select d).ToListAsync();

            return detalles;
        }
    }
}