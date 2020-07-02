using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        private readonly DataContext _context;
        public UsuarioRepository(DataContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<Usuario> ObtenerUsuario(int id)
        {
            return await _context.Usuario.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<Usuario>> ObtenerUsuarios()
        {
            return await _context.Usuario.ToListAsync();
        }
    }
}