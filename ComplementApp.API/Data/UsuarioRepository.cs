using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using System.Linq;
using System;
using ComplementApp.API.Helpers;
using System.Text;
using System.Collections.Generic;

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
                        .Include(c => c.Cargo)
                        .FirstOrDefaultAsync(u => u.UsuarioId == id);
        }

        public async Task<ICollection<Perfil>> ObtenerPerfilesxUsuario(int usuarioId)
        {
            List<Perfil> lista = new List<Perfil>();
            var perfiles =  await (    from up in _context.UsuarioPerfil
                                            join p in _context.Perfil on up.PerfilId equals p.PerfilId
                                            where up.UsuarioId == usuarioId
                                            select p).ToListAsync();

            return perfiles;
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

            var users = _context.Usuario.OrderBy(x => x.Nombres);
            
            return await PagedList<Usuario>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Usuario.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }

        public async Task<ICollection<Transaccion>> ObtenerListaTransaccionXUsuario(int usuarioId)
        {
            var transaciones = await (from up in _context.UsuarioPerfil
                                      join p in _context.Perfil on up.PerfilId equals p.PerfilId
                                      join pt in _context.PerfilTransaccion on p.PerfilId equals pt.PerfilId
                                      join t in _context.Transaccion on pt.TransaccionId equals t.TransaccionId
                                      where up.UsuarioId == usuarioId
                                      select t)
                                      .Distinct()
                                      .ToListAsync();
            return transaciones;
        }

        public bool RegistrarPerfilesAUsuario(int usuarioId, ICollection<Perfil> listaPerfiles)
        {

            UsuarioPerfil nuevoItem = null;
            List<UsuarioPerfil> lista = new List<UsuarioPerfil>();

            #region Eliminar relaciones

            var listaExistente = _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId);
            _context.UsuarioPerfil.RemoveRange(listaExistente);
            _unitOfWork.Complete();

            #endregion

            #region Setear datos

            foreach (var item in listaPerfiles)
            {
                nuevoItem = new UsuarioPerfil();
                nuevoItem.UsuarioId = usuarioId;
                nuevoItem.PerfilId = item.PerfilId;
                lista.Add(nuevoItem);
            }

            #endregion Setear datos

            _context.BulkInsert(lista);
            return true;
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