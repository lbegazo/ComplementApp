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
                        .Include(x => x.Area)
                        .Include(c => c.Cargo)
                        .FirstOrDefaultAsync(u => u.UsuarioId == id);
        }

        public async Task<ICollection<Perfil>> ObtenerPerfilesxUsuario(int usuarioId)
        {
            List<Perfil> lista = new List<Perfil>();
            var perfiles = await (from up in _context.UsuarioPerfil
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
            List<Transaccion> listaTransaccion = new List<Transaccion>();

            var transacciones = await (from up in _context.UsuarioPerfil
                                       join p in _context.Perfil on up.PerfilId equals p.PerfilId
                                       join pt in _context.PerfilTransaccion on p.PerfilId equals pt.PerfilId
                                       join t in _context.Transaccion on pt.TransaccionId equals t.TransaccionId
                                       where up.UsuarioId == usuarioId
                                             && t.Estado == true
                                       select t)
                        .Distinct()
                        .ToListAsync();

            foreach (var item in transacciones)
            {
                var children = GetChildren(transacciones, item.TransaccionId);

                if (children != null && children.ToList().Count > 0)
                {
                    item.Hijos = children.ToList();
                }

                if (item.PadreTransaccionId == 0)
                {
                    listaTransaccion.Add(item);
                }
            }

            List<Transaccion> lista = EliminarHijosConfundidos(listaTransaccion);

            return lista.ToList();
        }

        private List<Transaccion> EliminarHijosConfundidos(List<Transaccion> lista)
        {
            List<Transaccion> listaFinal = new List<Transaccion>();
            List<Transaccion> listaHijo = null;
            foreach (var item in lista)
            {
                if (item.Hijos != null && item.Hijos.Count > 0)
                {
                    listaHijo = new List<Transaccion>();
                    foreach (var hijo in item.Hijos)
                    {
                        if (item.TransaccionId == hijo.PadreTransaccionId)
                        {
                            listaHijo.Add(hijo);
                        }
                    }
                    item.Hijos = listaHijo;
                }

                listaFinal.Add(item);
            }
            return listaFinal;
        }

        List<Transaccion> GetChildren(List<Transaccion> foos, int id)
        {
            var query = foos.Where(x => x.PadreTransaccionId == id)
                            .Union(foos.Where(x => x.PadreTransaccionId == id)
                                .SelectMany(y => GetChildren(foos, y.TransaccionId))
                            ).ToList();

            var resultado = query.Where(x => x.TransaccionId != x.PadreTransaccionId);

            return query;
        }

        public List<Transaccion> Recursive(List<Transaccion> comments, int parentId)
        {
            List<Transaccion> inner = new List<Transaccion>();
            foreach (var t in comments.Where(c => c.PadreTransaccionId == parentId).ToList())
            {
                inner.Add(t);
                inner = inner.Union(Recursive(comments, t.TransaccionId)).ToList();
            }

            return inner;
        }

        public bool RegistrarPerfilesAUsuario(int usuarioId, ICollection<Perfil> listaPerfiles)
        {
            UsuarioPerfil nuevoItem = null;
            List<UsuarioPerfil> lista = new List<UsuarioPerfil>();

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

        public async Task<Transaccion> ObtenerTransaccionXCodigo(string codigoTransaccion)
        {
            Transaccion inner = await _context.Transaccion.Where(t => t.Codigo == codigoTransaccion).FirstOrDefaultAsync();
            return inner;
        }
       
       public bool EliminarPerfilesUsuario(int usuarioId)
        {
            var listaExistente = _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId).ToList();
            _context.UsuarioPerfil.RemoveRange(listaExistente);
            //_unitOfWork.Complete();
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