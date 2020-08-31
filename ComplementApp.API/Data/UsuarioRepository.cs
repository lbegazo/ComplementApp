using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using System.Linq;
using System;
using ComplementApp.API.Helpers;
using System.Text;

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
            return await _context.Usuario
                        .Include(x => x.Area)
                        .Include(c => c.Cargo).FirstOrDefaultAsync(u => u.UsuarioId == id);
        }

        public async Task<bool> EliminarUsuario(int id)
        {
            try
            {
                return await _context.Usuario.Where(x => x.UsuarioId == id).BatchDeleteAsync() > 0;

            }
            catch (Exception ex)
            {
                throw new Exception("error " + ex.Message);
            }
        }

        public async Task<PagedList<Usuario>> ObtenerUsuarios(UserParams userParams)
        {
            // return await _context.Usuario
            //                     .OrderBy(x => x.Nombres)
            //                     .ToListAsync();

            var users = _context.Usuario;
            return await PagedList<Usuario>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Usuario.AnyAsync(x => x.Username == username))
                return true;

            return false;
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