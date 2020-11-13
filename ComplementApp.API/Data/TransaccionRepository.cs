using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly DataContext _context;
        public TransaccionRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<Transaccion> ObtenerTransaccionXCodigo(string codigoTransaccion)
        {
            Transaccion inner = await _context.Transaccion.Where(t => t.Codigo == codigoTransaccion).FirstOrDefaultAsync();
            return inner;
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

        public bool EliminarPerfilesUsuario(int usuarioId)
        {
            var listaExistente = _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId).ToList();
            _context.UsuarioPerfil.RemoveRange(listaExistente);
            return true;
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

        public bool EliminarTransaccionesXPerfil(int perfilId)
        {
            var listaExistente = _context.PerfilTransaccion.Where(x => x.PerfilId == perfilId).ToList();
            _context.PerfilTransaccion.RemoveRange(listaExistente);
            return true;
        }

        public bool InsertarTransaccionesXPerfil(int perfilId, List<int> listaTransaccion)
        {
            PerfilTransaccion nuevoItem = null;
            List<PerfilTransaccion> lista = new List<PerfilTransaccion>();

            #region Setear datos

            foreach (var item in listaTransaccion)
            {
                nuevoItem = new PerfilTransaccion();
                nuevoItem.PerfilId = perfilId;
                nuevoItem.TransaccionId = item;
                lista.Add(nuevoItem);
            }

            #endregion Setear datos

            _context.BulkInsert(lista);
            return true;
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

        private List<Transaccion> GetChildren(List<Transaccion> foos, int id)
        {
            var query = foos.Where(x => x.PadreTransaccionId == id)
                            .Union(foos.Where(x => x.PadreTransaccionId == id)
                                .SelectMany(y => GetChildren(foos, y.TransaccionId))
                            ).ToList();

            var resultado = query.Where(x => x.TransaccionId != x.PadreTransaccionId);

            return query;
        }

        private List<Transaccion> Recursive(List<Transaccion> comments, int parentId)
        {
            List<Transaccion> inner = new List<Transaccion>();
            foreach (var t in comments.Where(c => c.PadreTransaccionId == parentId).ToList())
            {
                inner.Add(t);
                inner = inner.Union(Recursive(comments, t.TransaccionId)).ToList();
            }

            return inner;
        }

    }
}