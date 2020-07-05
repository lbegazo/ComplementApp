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
        public async Task<IEnumerable<CDP>> ObtenerCDPsXFiltro(int numeroCDP)
        {

            return await _context.CDP
                            .Where(x => x.Cdp == numeroCDP).ToListAsync();
        }

        public async Task<IEnumerable<DetalleCDP>> ObtenerItemsCDPxFiltro(string usuario, int numeroCDP)
        {
             return await _context.DetalleCDP
                            .Where(x => x.Cdp == numeroCDP)
                            .Where(x =>x.Responsable == usuario)
                            .ToListAsync();
        }
    }
}