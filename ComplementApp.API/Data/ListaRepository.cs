using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    

    public class ListaRepository: IListaRepository
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
    }
}