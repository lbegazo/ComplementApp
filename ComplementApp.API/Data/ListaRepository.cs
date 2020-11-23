using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Dtos;

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

        public async Task<ICollection<Estado>> ObtenerListaEstado(string tipoDocumento)
        {
            return await _context.Estado.Where(x => x.TipoDocumento == tipoDocumento).ToListAsync();
        }

        public async Task<IEnumerable<UsuarioParaDetalleDto>> ObtenerListaUsuarioxFiltro(string nombres)
        {
            var listaUsuario = await (from u in _context.Usuario
                                      where (u.Nombres.Contains(nombres))
                                      select new UsuarioParaDetalleDto()
                                      {
                                          UsuarioId = u.UsuarioId,
                                          Nombres = u.Nombres,
                                          Apellidos = u.Apellidos,
                                          NombreCompleto = u.Nombres + " " + u.Apellidos,
                                          Username = u.Username
                                      }).ToListAsync();

            foreach (var usuario in listaUsuario)
            {
                Perfil perfil = await (from up in _context.UsuarioPerfil
                                       join p in _context.Perfil on up.PerfilId equals p.PerfilId
                                       where up.UsuarioId == usuario.UsuarioId
                                       select p).FirstOrDefaultAsync();
                usuario.Perfiles = new List<Perfil>();
                usuario.Perfiles.Add(perfil);
            }
            return listaUsuario;

        }

    }
}