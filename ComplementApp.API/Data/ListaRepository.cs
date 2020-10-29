using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ComplementApp.API.Interfaces;

namespace ComplementApp.API.Data
{
    public class ListaRepository : IListaRepository
    {
        private readonly DataContext _context;
        public ListaRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Cargo>> ObtenerCargos()
        {
            return await _context.Cargo.ToListAsync();
        }

        public async Task<IEnumerable<Area>> ObtenerAreas()
        {
            return await _context.Area.ToListAsync();
        }

        public async Task<IEnumerable<TipoOperacion>> ObtenerListaTipoOperacion()
        {
            return await _context.TipoOperacion.ToListAsync();
        }

        public async Task<IEnumerable<TipoDetalleCDP>> ObtenerListaTipoDetalleModificacion()
        {
            return await _context.TipoDetalleModificacion.ToListAsync();
        }

        public async Task<IEnumerable<Tercero>> ObtenerListaTercero(string numeroIdentificacion)
        {
            return await _context.Tercero
                            .Where(t => t.NumeroIdentificacion.Contains(numeroIdentificacion))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Perfil>> ObtenerListaPerfiles()
        {
            return await _context.Perfil
                        .Where(x => x.Estado == true)
                        .ToListAsync();
        }

        public async Task<IEnumerable<ParametroGeneral>> ObtenerParametrosGenerales()
        {
            return await _context.ParametroGeneral.ToListAsync();
        }

        public async Task<ParametroLiquidacionTercero> ObtenerParametroLiquidacionXTercero(int terceroId)
        {
            return await _context.ParametroLiquidacionTercero
                        .Where(x => x.TerceroId == terceroId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<CriterioCalculoReteFuente>> ObtenerListaCriterioCalculoReteFuente()
        {
            return await _context.CriterioCalculoReteFuente.ToListAsync();
        }
        
    }
}