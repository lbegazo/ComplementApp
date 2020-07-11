using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public UsuarioRepository(DataContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _unitOfWork = unitOfWork;
            this._context = context;
        }

        public async Task<Usuario> Register(Usuario user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Usuario.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return user;
        }

        public async Task<Usuario> ObtenerUsuario(int id)
        {
            return await _context.Usuario.Include(x => x.Area).Include(c => c.Cargo).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<Usuario>> ObtenerUsuarios()
        {
            return await _context.Usuario.ToListAsync();
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Usuario.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }

        public async Task<IEnumerable<Cargo>> ObtenerCargos()
        {
            return await _context.Cargo.ToListAsync();
        }

        public async Task<IEnumerable<Area>> ObtenerAreas()
        {
            return await _context.Area.ToListAsync();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}