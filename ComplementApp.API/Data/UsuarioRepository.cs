using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using System.Linq;
using System;
using ComplementApp.API.Helpers;
using System.Text;
using System.Collections.Generic;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces.Repository;

namespace ComplementApp.API.Data
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public UsuarioRepository(DataContext context, IUnitOfWork unitOfWork)
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

        public async Task<Usuario> ObtenerUsuarioBase(int id)
        {
            return await _context.Usuario
                        // .Include(x => x.Area)
                        // .Include(c => c.Cargo)
                        .FirstOrDefaultAsync(u => u.UsuarioId == id);
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

        public async Task<PagedList<UsuarioParaDetalleDto>> ObtenerUsuarios(int tipo, int? usuarioId, UserParams userParams)
        {
            IOrderedQueryable<UsuarioParaDetalleDto> lista = null;

            lista = (from t in _context.Usuario
                     join ar in _context.Area on t.AreaId equals ar.AreaId
                     join ca in _context.Cargo on t.CargoId equals ca.CargoId
                     where t.UsuarioId == usuarioId || usuarioId == null
                     where t.PciId == userParams.PciId
                     select new UsuarioParaDetalleDto()
                     {
                         UsuarioId = t.UsuarioId,
                         Nombres = t.Nombres,
                         Apellidos = t.Apellidos,
                         CargoNombre = ca.Nombre,
                         AreaNombre = ar.Nombre,
                         Username = t.Username,
                     })
                       .Distinct()
                       .OrderBy(t => t.Nombres);


            return await PagedList<UsuarioParaDetalleDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Usuario.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }

        public async Task<ICollection<ValorSeleccion>> ObtenerListaUsuarioXPerfil(int perfilId, int pciId)
        {
            List<ValorSeleccion> lista = new List<ValorSeleccion>();

            var lista1 = (from u in _context.Usuario
                          join pu in _context.UsuarioPerfil on u.UsuarioId equals pu.UsuarioId
                          where u.PciId == pciId
                          where pu.PerfilId == perfilId
                          select new ValorSeleccion()
                          {
                              Id = u.UsuarioId,
                              Nombre = u.Nombres + ' ' + u.Apellidos,
                          });

            lista = await lista1.OrderBy(x => x.Nombre).ToListAsync();

            return lista;
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